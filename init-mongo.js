// MongoDB Init Script for Marketplace
// Creates databases and initial collections

// Switch to admin database
db = db.getSiblingDB('admin');

// Create marketplace_order database
db = db.getSiblingDB('marketplace_order');

// Create orders collection
db.createCollection('orders', {
  validator: {
    $jsonSchema: {
      bsonType: 'object',
      required: ['orderNumber', 'userId', 'status', 'createdAt'],
      properties: {
        _id: {
          bsonType: 'objectId'
        },
        orderNumber: {
          bsonType: 'string'
        },
        userId: {
          bsonType: 'string'
        },
        userName: {
          bsonType: 'string'
        },
        totalAmount: {
          bsonType: 'decimal'
        },
        status: {
          enum: ['Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled', 'Refunded']
        },
        orderItems: {
          bsonType: 'array',
          items: {
            bsonType: 'object',
            properties: {
              productId: { bsonType: 'string' },
              productName: { bsonType: 'string' },
              quantity: { bsonType: 'int' },
              unitPrice: { bsonType: 'decimal' },
              totalPrice: { bsonType: 'decimal' }
            }
          }
        },
        shippingAddress: {
          bsonType: 'object',
          properties: {
            street: { bsonType: 'string' },
            city: { bsonType: 'string' },
            state: { bsonType: 'string' },
            country: { bsonType: 'string' },
            zipCode: { bsonType: 'string' },
            phoneNumber: { bsonType: 'string' }
          }
        },
        paymentInfo: {
          bsonType: 'object',
          properties: {
            cardName: { bsonType: 'string' },
            cardNumber: { bsonType: 'string' },
            expiration: { bsonType: 'string' },
            cvv: { bsonType: 'string' },
            paymentMethod: { enum: ['CreditCard', 'DebitCard', 'PayPal', 'BankTransfer'] }
          }
        },
        createdAt: {
          bsonType: 'date'
        },
        updatedAt: {
          bsonType: 'date'
        }
      }
    }
  }
});

// Create indexes
db.orders.createIndex({ userId: 1 });
db.orders.createIndex({ orderNumber: 1 }, { unique: true });
db.orders.createIndex({ status: 1 });
db.orders.createIndex({ createdAt: -1 });
db.orders.createIndex({ userId: 1, createdAt: -1 });

print('✅ marketplace_order database initialized');
print('✅ orders collection created with validation');
print('✅ Indexes created for performance');
