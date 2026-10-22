export interface Client {
  id?: string;
  name: string;
  email: string;
}

export interface Invoice {
  id?: string;
  customerId: string;
  amount: number;
  status: 'primerrecordatorio' | 'segundorecordatorio' | 'desactivado';
}