import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, Client } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5085/api';

  // Obtiene los clientes dinámicamente desde MongoDB via CustomersController
  getCustomers(): Observable<Client[]> {
    return this.http.get<Client[]>(`${this.apiUrl}/customers`);
  }

  // Obtiene las facturas de un cliente específico
  getInvoicesByClient(customerId: string): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(`${this.apiUrl}/invoices/customer/${customerId}`);
  }

  // Dispara el proceso de recordatorios y cambio de estados
  processReminders(customerId: string): Observable<{ message: string; customerId: string; processedInvoices: number }> {
    return this.http.post<{ message: string; customerId: string; processedInvoices: number }>(
      `${this.apiUrl}/invoices/process-reminders/${customerId}`,
      {}
    );
  }
}