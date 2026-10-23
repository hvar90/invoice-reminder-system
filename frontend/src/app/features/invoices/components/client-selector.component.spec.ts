import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClientSelectorComponent } from './client-selector.component';
import { Client } from '../../../core/models/invoice.model';
import { vi } from 'vitest';

describe('ClientSelectorComponent', () => {
  let component: ClientSelectorComponent;
  let fixture: ComponentFixture<ClientSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientSelectorComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ClientSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render clients in the select dropdown', () => {
    const mockClients: Client[] = [
      { id: '1', name: 'Maria Gomez', email: 'maria@example.com' }
    ];
    component.clients = mockClients;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const options = compiled.querySelectorAll('option');
    // Opción por defecto + 1 cliente
    expect(options.length).toBe(2);
    expect(options[1].textContent).toContain('Maria Gomez');
  });

  it('should emit clientSelected event when selection changes', () => {
    vi.spyOn(component.clientSelected, 'emit');
    
    component.onSelectionChange('1');

    expect(component.clientSelected.emit).toHaveBeenCalledWith('1');
  });
});