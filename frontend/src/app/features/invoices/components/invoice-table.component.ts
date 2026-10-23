import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Invoice } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-invoice-table',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h3 style="margin-top: 24px; margin-bottom: 12px; font-size: 1.1rem;">Invoices for Selected Client</h3>
    
    <div style="width: 100%; overflow-x: auto; -webkit-overflow-scrolling: touch; border: 1px solid #ddd; border-radius: 8px;">
      <table style="width: 100%; border-collapse: collapse; min-width: 320px; text-align: left; font-size: 14px;">
        <thead>
          <tr style="background-color: #f8f9fa; border-bottom: 2px solid #dee2e6;">
            <th style="padding: 10px 12px; white-space: nowrap;">Invoice ID</th>
            <th style="padding: 10px 12px; white-space: nowrap;">Amount</th>
            <th style="padding: 10px 12px; white-space: nowrap;">Status</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let invoice of invoices" style="border-bottom: 1px solid #e9ecef;">
            <td style="padding: 10px 12px; font-family: monospace; word-break: break-all; font-size: 12px;">{{ invoice.id }}</td>
            <td style="padding: 10px 12px; white-space: nowrap; font-weight: 500;">$ {{ invoice.amount | number:'1.2-2' }}</td>
            <td style="padding: 10px 12px; min-width: 130px;">
              <span [style.color]="getStatusColor(invoice.status)" style="font-weight: bold; word-break: break-word; display: inline-block;">
                {{ formatStatus(invoice.status) }}
              </span>
            </td>
          </tr>
          <tr *ngIf="invoices.length === 0">
            <td colspan="3" style="text-align: center; padding: 16px; color: #6c757d;">No invoices found for this client.</td>
          </tr>
        </tbody>
      </table>
    </div>
  `
})
export class InvoiceTableComponent {
  @Input() invoices: Invoice[] = [];

  // Método para formatear el texto técnico y hacerlo más legible
  formatStatus(status: string): string {
    switch (status) {
      case 'primerrecordatorio': return 'Primer Recordatorio';
      case 'segundorecordatorio': return 'Segundo Recordatorio';
      case 'desactivado': return 'Desactivado';
      default: return status;
    }
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