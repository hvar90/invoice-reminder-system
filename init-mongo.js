db = db.getSiblingDB('GestionFacturas');

db.createCollection('Facturas');

db.Facturas.insertMany([
  {
    clienteId: "CLI01",
    nombreCliente: "Carlos Perez",
    emailCliente: "carlos@example.com",
    monto: 150.00,
    estado: "primerrecordatorio"
  },
  {
    clienteId: "CLI02",
    nombreCliente: "Maria Gomez",
    emailCliente: "maria@example.com",
    monto: 300.50,
    estado: "segundorecordatorio"
  },
  {
    clienteId: "CLI03",
    nombreCliente: "Juan Rodriguez",
    emailCliente: "juan@example.com",
    monto: 80.00,
    estado: "desactivado"
  }
]);