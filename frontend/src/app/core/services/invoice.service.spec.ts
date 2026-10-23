import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { InvoiceService } from './invoice.service';
import { Client, Invoice } from '../models/invoice.model';

describe('InvoiceService', () => {
  let service: InvoiceService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5085/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        InvoiceService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(InvoiceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch customers via GET', () => {
    const mockClients: Client[] = [
      { id: '1', name: 'Carlos Perez', email: 'carlos@example.com' }
    ];

    service.getCustomers().subscribe(clients => {
      expect(clients.length).toBe(1);
      expect(clients).toEqual(mockClients);
    });

    const req = httpMock.expectOne(`${apiUrl}/customers`);
    expect(req.request.method).toBe('GET');
    req.flush(mockClients);
  });

  it('should fetch invoices by client ID via GET', () => {
    const clientId = '1';
    const mockInvoices: Invoice[] = [
      { id: 'inv1', customerId: '1', amount: 150, status: 'primerrecordatorio' }
    ];

    service.getInvoicesByClient(clientId).subscribe(invoices => {
      expect(invoices.length).toBe(1);
      expect(invoices).toEqual(mockInvoices);
    });

    const req = httpMock.expectOne(`${apiUrl}/invoices/customer/${clientId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockInvoices);
  });

  it('should process reminders via POST', () => {
    const clientId = '1';
    const mockResponse = { message: 'Success', customerId: '1', processedInvoices: 2 };

    service.processReminders(clientId).subscribe(res => {
      expect(res.message).toBe('Success');
      expect(res.processedInvoices).toBe(2);
    });

    const req = httpMock.expectOne(`${apiUrl}/invoices/process-reminders/${clientId}`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });
});