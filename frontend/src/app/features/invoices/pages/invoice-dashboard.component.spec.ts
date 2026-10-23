import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InvoiceDashboardComponent } from './invoice-dashboard.component';
import { InvoiceService } from '../../../core/services/invoice.service';
import { of, delay } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { vi } from 'vitest';

describe('InvoiceDashboardComponent', () => {
  let component: InvoiceDashboardComponent;
  let fixture: ComponentFixture<InvoiceDashboardComponent>;
  let invoiceServiceSpy: any;

  const mockClients = [
    { id: '1', name: 'Carlos Perez', email: 'carlos@example.com' }
  ];
  const mockInvoices = [
    { id: 'inv1', customerId: '1', amount: 150, status: 'primerrecordatorio' as const }
  ];

  beforeEach(async () => {
    const spy = {
      getCustomers: vi.fn().mockReturnValue(of(mockClients)),
      getInvoicesByClient: vi.fn().mockReturnValue(of(mockInvoices)),
      // Usamos delay(1) para que la respuesta sea asíncrona y evite conflictos de ciclo en Angular
      processReminders: vi.fn().mockReturnValue(
        of({ message: 'OK', customerId: '1', processedInvoices: 1 }).pipe(delay(1))
      )
    };

    await TestBed.configureTestingModule({
      imports: [InvoiceDashboardComponent],
      providers: [
        { provide: InvoiceService, useValue: spy },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceDashboardComponent);
    component = fixture.componentInstance;
    invoiceServiceSpy = TestBed.inject(InvoiceService);
    fixture.detectChanges();
  });

  it('should create and load customers on init', () => {
    expect(component).toBeTruthy();
    expect(invoiceServiceSpy.getCustomers).toHaveBeenCalled();
    expect(component.clients().length).toBe(1);
    expect(component.clients()[0].name).toBe('Carlos Perez');
  });

  it('should handle client selection and load invoices', () => {
    component.onClientSelected('1');
    fixture.detectChanges();

    expect(component.selectedClientId()).toBe('1');
    expect(invoiceServiceSpy.getInvoicesByClient).toHaveBeenCalledWith('1');
    expect(component.invoices().length).toBe(1);
  });

  it('should process reminders and display success message with client name', async () => {
    component.onClientSelected('1');
    fixture.detectChanges();

    component.processReminders();
    
    // Esperamos a que se complete el flujo asíncrono del observable
    await new Promise(resolve => setTimeout(resolve, 20));
    fixture.detectChanges();

    expect(invoiceServiceSpy.processReminders).toHaveBeenCalledWith('1');
    expect(component.message).toBe('Proceso finalizado para Carlos Perez');
    expect(component.processing).toBe(false);
  });
});