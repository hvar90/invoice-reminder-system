import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { Subject, switchMap, of } from 'rxjs';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ClientSelectorComponent } from '../components/client-selector.component';
import { InvoiceTableComponent } from '../components/invoice-table.component';

@Component({
  selector: 'app-invoice-dashboard',
  standalone: true,
  imports: [CommonModule, ClientSelectorComponent, InvoiceTableComponent],
  template: `
    <div style="padding: 16px; max-width: 900px; margin: 0 auto; font-family: system-ui, -apple-system, sans-serif; box-sizing: border-box;">
      <h2 style="font-size: 1.4rem; margin-bottom: 20px; word-break: break-word;">Invoice Management & Reminder System</h2>

      <!-- Selector de Clientes -->
      <app-client-selector 
        [clients]="clients()" 
        [selectedClientId]="selectedClientId()" 
        (clientSelected)="onClientSelected($event)">
      </app-client-selector>

      <!-- Botón de Acción Responsive -->
      <div style="margin-bottom: 20px;" *ngIf="selectedClientId()">
        <button 
          (click)="processReminders()" 
          [disabled]="processing" 
          style="padding: 12px 18px; background-color: #007bff; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 14px; font-weight: 500; width: 100%; max-width: 350px;">
          {{ processing ? 'Processing...' : 'Process Reminders & Send Emails' }}
        </button>
        <div *ngIf="message" style="margin-top: 10px; color: #28a745; font-weight: bold; font-size: 14px;">
          {{ message }}
        </div>
      </div>

      <!-- Tabla de Facturas -->
      <div *ngIf="selectedClientId()">
        <app-invoice-table [invoices]="invoices()"></app-invoice-table>
      </div>
    </div>
  `
})
export class InvoiceDashboardComponent {
  private invoiceService = inject(InvoiceService);

  clients = toSignal(this.invoiceService.getCustomers(), { initialValue: [] });

  private selectedClientSubject = new Subject<string>();
  selectedClientId = toSignal(this.selectedClientSubject, { initialValue: '' });

  invoices = toSignal(
    this.selectedClientSubject.pipe(
      switchMap(clientId => clientId ? this.invoiceService.getInvoicesByClient(clientId) : of([]))
    ),
    { initialValue: [] }
  );

  processing: boolean = false;
  message: string = '';

  onClientSelected(clientId: string): void {
    this.message = '';
    this.selectedClientSubject.next(clientId);
  }

  processReminders(): void {
    const clientId = this.selectedClientId();
    if (!clientId) return;
    
    this.processing = true;
    this.message = '';
    const currentClient = this.clients().find(c => c.id === clientId);
    const clientName = currentClient ? currentClient.name : 'Cliente';

    this.invoiceService.processReminders(clientId).subscribe({
      next: (response: { message?: string }) => {
        this.processing = false;
        this.message = `Proceso finalizado para ${clientName}`;
        this.selectedClientSubject.next(clientId);
      },
      error: (err: unknown) => {
        this.processing = false;
        console.error('Error processing reminders', err);
        this.message = 'Error processing reminders.';
      }
    });
  }
}