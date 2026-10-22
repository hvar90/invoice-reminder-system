db = db.getSiblingDB('GestionFacturas');

db.Clientes.drop();
db.Facturas.drop();

// Define los ObjectIds únicos para los 3 clientes
var cli1 = new ObjectId("650000000000000000000001");
var cli2 = new ObjectId("650000000000000000000002");
var cli3 = new ObjectId("650000000000000000000003");

// Insertar 3 Clientes
db.Clientes.insertMany([
  {
    _id: cli1,
    nombre: "Carlos Perez",
    email: "carlos@example.com"
  },
  {
    _id: cli2,
    nombre: "Maria Gomez",
    email: "maria@example.com"
  },
  {
    _id: cli3,
    nombre: "Juan Rodriguez",
    email: "juan@example.com"
  }
]);

// Insertar 5 Facturas por cada cliente (15 en total)
db.Facturas.insertMany([
  // Facturas del Cliente 1 (Carlos Perez)
  { clienteId: cli1, monto: 150.00, estado: "primerrecordatorio" },
  { clienteId: cli1, monto: 200.50, estado: "segundorecordatorio" },
  { clienteId: cli1, monto: 85.00,  estado: "primerrecordatorio" },
  { clienteId: cli1, monto: 450.00, estado: "desactivado" },
  { clienteId: cli1, monto: 120.00, estado: "primerrecordatorio" },

  // Facturas del Cliente 2 (Maria Gomez)
  { clienteId: cli2, monto: 300.50, estado: "segundorecordatorio" },
  { clienteId: cli2, monto: 95.00,  estado: "primerrecordatorio" },
  { clienteId: cli2, monto: 500.00, estado: "desactivado" },
  { clienteId: cli2, monto: 210.00, estado: "segundorecordatorio" },
  { clienteId: cli2, monto: 175.25, estado: "primerrecordatorio" },

  // Facturas del Cliente 3 (Juan Rodriguez)
  { clienteId: cli3, monto: 80.00,  estado: "desactivado" },
  { clienteId: cli3, monto: 130.00, estado: "primerrecordatorio" },
  { clienteId: cli3, monto: 620.00, estado: "segundorecordatorio" },
  { clienteId: cli3, monto: 90.50,  estado: "primerrecordatorio" },
  { clienteId: cli3, monto: 310.00, estado: "desactivado" }
]);