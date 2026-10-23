import { Routes } from '@angular/router';
import { InvoiceDashboardComponent } from './features/invoices/pages/invoice-dashboard.component';

export const routes: Routes = [
  { path: '', component: InvoiceDashboardComponent },
  { path: '**', redirectTo: '' } // Redirige cualquier ruta desconocida al inicio
];