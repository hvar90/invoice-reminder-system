import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { Subject, switchMap, startWith, of, tap } from 'rxjs';
import { InvoiceService } from '../../../core/services/invoice.service';
import { Invoice, Client } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-invoice-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="padding: 24px; font-family: Arial, sans-serif;">
      <h2>Invoice Management & Reminder System</h2>

      <!-- Client Combobox -->
      <div style="margin-bottom: 20px;">
        <label for="clientSelect" style="font-weight: bold; margin-right: 10px;">Select Client:</label>
        <!-- Actualizamos el modelo usando un Subject o FormControl reactivo -->
        <select id="clientSelect" [ngModel]="selectedClientId()" (ngModelChange)="onClientSelected($event)" style="padding: 8px; width: 320px;">
          <option value="" disabled selected>-- Select a client --</option>
          <option *ngFor="let client of clients()" [value]="client.id">
            {{ client.name }} ({{ client.email }})
          </option>
        </select>
      </div>

      <!-- Process Reminders Action Button -->
      <div style="margin-bottom: 20px;" *ngIf="selectedClientId()">
        <button (click)="processReminders()" [disabled]="processing" style="padding: 10px 16px; background-color: #007bff; color: white; border: none; border-radius: 4px; cursor: pointer;">
          {{ processing ? 'Processing...' : 'Process Reminders & Send Emails' }}
        </button>
        <span *ngIf="message" style="margin-left: 15px; color: #28a745; font-weight: bold;">{{ message }}</span>
      </div>

      <!-- Invoices Table -->
      <div *ngIf="selectedClientId()">
        <h3>Invoices for Selected Client</h3>
        <table border="1" cellpadding="10" cellspacing="0" style="width: 100%; border-collapse: collapse;">
          <thead>
            <tr style="background-color: #f2f2f2;">
              <th>Invoice ID</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let invoice of invoices()">
              <td>{{ invoice.id }}</td>
              <td>$ {{ invoice.amount | number:'1.2-2' }}</td>
              <td>
                <span [style.color]="getStatusColor(invoice.status)" style="font-weight: bold;">
                  {{ invoice.status }}
                </span>
              </td>
            </tr>
            <tr *ngIf="invoices().length === 0">
              <td colspan="3" style="text-align: center;">No invoices found for this client.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class InvoiceDashboardComponent {
  private invoiceService = inject(InvoiceService);

  // 1. Clientes convertidos directamente a Signal desde el Observable del servicio
  clients = toSignal(this.invoiceService.getCustomers(), { initialValue: [] });

  // 2. Un Subject que emite cada vez que el usuario cambia de cliente
  private selectedClientSubject = new Subject<string>();
  selectedClientId = toSignal(this.selectedClientSubject, { initialValue: '' });

  // 3. Facturas automatizadas: Cada vez que 'selectedClientId' cambia, dispara la petición HTTP sola
  invoices = toSignal(
    this.selectedClientSubject.pipe(
      switchMap(clientId => clientId ? this.invoiceService.getInvoicesByClient(clientId) : of([]))
    ),
    { initialValue: [] }
  );

  processing: boolean = false;
  message: string = '';

  // Método disparado por el cambio en el HTML
  onClientSelected(clientId: string): void {
    this.message = '';
    this.selectedClientSubject.next(clientId); // Emitimos el nuevo ID al flujo reactivo
  }

  processReminders(): void {
    const clientId = this.selectedClientId();
    if (!clientId) return;
    
    this.processing = true;
    this.message = '';

    this.invoiceService.processReminders(clientId).subscribe({
      next: (response: { message?: string }) => {
        this.processing = false;
        this.message = response?.message || 'Process completed successfully!';
        // Forzamos una re-emisión en el subject para recargar las facturas del mismo cliente
        this.selectedClientSubject.next(clientId);
      },
      error: (err: unknown) => {
        this.processing = false;
        console.error('Error processing reminders', err);
        this.message = 'Error processing reminders.';
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'primerrecordatorio': return '#d9534f';
      case 'segundorecordatorio': return '#f0ad4e';
      case 'desactivado': return '#6c757d';
      default: return '#28a745';
    }
  }
}