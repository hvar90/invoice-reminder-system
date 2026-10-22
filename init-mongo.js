db = db.getSiblingDB('InvoiceManagement');

db.Customers.drop();
db.Invoices.drop();

// Define unique ObjectIds for the 3 customers
var cust1 = new ObjectId("650000000000000000000001");
var cust2 = new ObjectId("650000000000000000000002");
var cust3 = new ObjectId("650000000000000000000003");

// Insert 3 Customers
db.Customers.insertMany([
  {
    _id: cust1,
    name: "Carlos Perez",
    email: "carlos@example.com"
  },
  {
    _id: cust2,
    name: "Maria Gomez",
    email: "maria@example.com"
  },
  {
    _id: cust3,
    name: "Juan Rodriguez",
    email: "juan@example.com"
  }
]);

// Insert 5 Invoices for each customer (15 in total)
db.Invoices.insertMany([
  // Invoices for Customer 1 (Carlos Perez)
  { customerId: cust1, amount: 150.00, status: "primerrecordatorio" },
  { customerId: cust1, amount: 200.50, status: "segundorecordatorio" },
  { customerId: cust1, amount: 85.00,  status: "primerrecordatorio" },
  { customerId: cust1, amount: 450.00, status: "desactivado" },
  { customerId: cust1, amount: 120.00, status: "primerrecordatorio" },

  // Invoices for Customer 2 (Maria Gomez)
  { customerId: cust2, amount: 300.50, status: "segundorecordatorio" },
  { customerId: cust2, amount: 95.00,  status: "primerrecordatorio" },
  { customerId: cust2, amount: 500.00, status: "desactivado" },
  { customerId: cust2, amount: 210.00, status: "segundorecordatorio" },
  { customerId: cust2, amount: 175.25, status: "primerrecordatorio" },

  // Invoices for Customer 3 (Juan Rodriguez)
  { customerId: cust3, amount: 80.00,  status: "desactivado" },
  { customerId: cust3, amount: 130.00, status: "primerrecordatorio" },
  { customerId: cust3, amount: 620.00, status: "segundorecordatorio" },
  { customerId: cust3, amount: 90.50,  status: "primerrecordatorio" },
  { customerId: cust3, amount: 310.00, status: "desactivado" }
]);