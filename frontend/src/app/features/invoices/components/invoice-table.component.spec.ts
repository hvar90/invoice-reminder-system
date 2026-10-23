import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InvoiceTableComponent } from './invoice-table.component';
import { Invoice } from '../../../core/models/invoice.model';

describe('InvoiceTableComponent', () => {
  let component: InvoiceTableComponent;
  let fixture: ComponentFixture<InvoiceTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceTableComponent]
    }).compileComponents();
  });

  it('should create', () => {
    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should format status correctly', () => {
    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    
    expect(component.formatStatus('primerrecordatorio')).toBe('Primer Recordatorio');
    expect(component.formatStatus('segundorecordatorio')).toBe('Segundo Recordatorio');
    expect(component.formatStatus('desactivado')).toBe('Desactivado');
    expect(component.formatStatus('desconocido')).toBe('desconocido');
  });

  it('should return correct status colors', () => {
    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    
    expect(component.getStatusColor('primerrecordatorio')).toBe('#d9534f');
    expect(component.getStatusColor('segundorecordatorio')).toBe('#f0ad4e');
    expect(component.getStatusColor('desactivado')).toBe('#6c757d');
    expect(component.getStatusColor('otro')).toBe('#28a745');
  });

  it('should display "No invoices found" when list is empty', () => {
    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    
    // Asignamos la lista vacía ANTES del primer detectChanges
    component.invoices = [];
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('No invoices found for this client.');
  });

  it('should render invoices when provided', () => {
    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;

    const mockInvoices: Invoice[] = [
      { id: 'inv1', customerId: '1', amount: 300.50, status: 'segundorecordatorio' }
    ];
    
    // Asignamos las facturas ANTES de que ocurra el primer detectChanges de esta prueba
    component.invoices = mockInvoices;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('inv1');
    expect(compiled.textContent).toContain('Segundo Recordatorio');
  });
});