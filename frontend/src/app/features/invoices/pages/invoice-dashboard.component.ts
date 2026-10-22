import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../../core/services/invoice.service';
import { Invoice, Client } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-invoice-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="padding: 24px; font-family: Arial, sans-serif;">
      <h2>Invoice Management & Reminder System</h2>

      <!-- Client Combobox (Dynamic from Backend / MongoDB) -->
      <div style="margin-bottom: 20px;">
        <label for="clientSelect" style="font-weight: bold; margin-right: 10px;">Select Client:</label>
        <select id="clientSelect" [(ngModel)]="selectedClientId" (change)="loadInvoices()" style="padding: 8px; width: 320px;">
          <option value="" disabled selected>-- Select a client --</option>
          <option *ngFor="let client of clients" [value]="client.id">
            {{ client.name }} ({{ client.email }})
          </option>
        </select>
      </div>

      <!-- Process Reminders Action Button -->
      <div style="margin-bottom: 20px;" *ngIf="selectedClientId">
        <button (click)="processReminders()" [disabled]="processing" style="padding: 10px 16px; background-color: #007bff; color: white; border: none; border-radius: 4px; cursor: pointer;">
          {{ processing ? 'Processing...' : 'Process Reminders & Send Emails' }}
        </button>
        <span *ngIf="message" style="margin-left: 15px; color: #28a745; font-weight: bold;">{{ message }}</span>
      </div>

      <!-- Invoices Table -->
      <div *ngIf="selectedClientId">
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
            <tr *ngFor="let invoice of invoices">
              <td>{{ invoice.id }}</td>
              <td>$ {{ invoice.amount | number:'1.2-2' }}</td>
              <td>
                <span [style.color]="getStatusColor(invoice.status)" style="font-weight: bold;">
                  {{ invoice.status }}
                </span>
              </td>
            </tr>
            <tr *ngIf="invoices.length === 0">
              <td colspan="3" style="text-align: center;">No invoices found for this client.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class InvoiceDashboardComponent implements OnInit {
  private invoiceService = inject(InvoiceService);

  clients: Client[] = [];
  selectedClientId: string = '';
  invoices: Invoice[] = [];
  processing: boolean = false;
  message: string = '';

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.invoiceService.getCustomers().subscribe({
      next: (data: Client[]) => {
        this.clients = data;
      },
      error: (err: unknown) => {
        console.error('Error loading customers', err);
      }
    });
  }

  loadInvoices(): void {
    if (!this.selectedClientId) return;
    this.message = '';
    this.invoiceService.getInvoicesByClient(this.selectedClientId).subscribe({
      next: (data: Invoice[]) => {
        this.invoices = data;
      },
      error: (err: unknown) => {
        console.error('Error loading invoices', err);
      }
    });
  }

  processReminders(): void {
    if (!this.selectedClientId) return;
    this.processing = true;
    this.message = '';

    this.invoiceService.processReminders(this.selectedClientId).subscribe({
      next: (response: { message?: string }) => {
        this.processing = false;
        this.message = response?.message || 'Process completed successfully!';
        this.loadInvoices(); // Recarga las facturas automáticamente para reflejar los nuevos estados
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