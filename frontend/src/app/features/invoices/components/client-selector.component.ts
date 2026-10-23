import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Client } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-client-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="margin-bottom: 20px; display: flex; flex-direction: column; gap: 8px;">
      <label for="clientSelect" style="font-weight: bold;">Select Client:</label>
      <select 
        id="clientSelect" 
        [ngModel]="selectedClientId" 
        (ngModelChange)="onSelectionChange($event)" 
        style="padding: 10px; width: 100%; max-width: 400px; box-sizing: border-box; border-radius: 6px; border: 1px solid #ccc; font-size: 14px;">
        <option value="" disabled selected>-- Select a client --</option>
        <option *ngFor="let client of clients" [value]="client.id">
          {{ client.name }} ({{ client.email }})
        </option>
      </select>
    </div>
  `
})
export class ClientSelectorComponent {
  @Input() clients: Client[] = [];
  @Input() selectedClientId: string = '';
  @Output() clientSelected = new EventEmitter<string>();

  onSelectionChange(clientId: string): void {
    this.clientSelected.emit(clientId);
  }
}