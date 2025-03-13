USE mams_db;

-- Insert data into products_types
INSERT INTO products_types (product_type_name, product_type_archive) VALUES
('Raw Honey', NULL),
('Creamed Honey', NULL),
('Infused Honey', NULL),
('Honey Comb', NULL),
('Propolis', NULL),
('Pollen', '2024-01-15 09:30:00'),
('Royal Jelly', NULL),
('Beeswax Products', NULL);

-- Insert data into products_categories
INSERT INTO products_categories (product_category_name, product_category_archive) VALUES
('Spring Collection', NULL),
('Summer Collection', NULL),
('Autumn Collection', NULL),
('Winter Collection', '2024-02-10 14:45:00'),
('Premium Collection', NULL),
('Organic Collection', NULL),
('Gift Collection', NULL),
('Natural Remedies', '2024-03-01 11:20:00');

-- Insert data into products_shapes
INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES
('Jar', NULL),
('Bottle', NULL),
('Hexagonal Jar', NULL),
('Square Jar', '2024-01-20 10:15:00'),
('Gift Box', NULL),
('Tube', NULL),
('Comb Frame', NULL),
('Honeycomb Section', NULL);

-- Insert data into beehives
INSERT INTO beehives (beehive_name, beehive_archive) VALUES
('Wildflower Meadow', NULL),
('Orchard Grove', NULL),
('Mountain Ridge', '2024-02-05 08:30:00'),
('Forest Edge', NULL),
('Lavender Fields', NULL),
('Riverside Hives', NULL),
('Valley Apiary', '2024-01-10 16:45:00'),
('Clover Fields', NULL);

-- Insert data into products_lots
INSERT INTO products_lots (product_lot_nbr, product_lot_year, fk_beehive_id, product_lot_archive) VALUES
(101, 2024, 1, NULL),
(102, 2024, 2, NULL),
(103, 2024, 3, NULL),
(104, 2024, 4, NULL),
(105, 2024, 5, '2024-02-15 13:20:00'),
(106, 2024, 6, NULL),
(107, 2024, 7, NULL),
(108, 2024, 8, NULL),
(201, 2023, 1, '2024-03-05 09:40:00'),
(202, 2023, 2, NULL);

-- Insert data into products
INSERT INTO products (product_name, product_weight, fk_product_category_id, fk_product_type_id, fk_product_shape_id, fk_product_lot_id, product_archive) VALUES
('Wildflower Raw Honey', 500, 1, 1, 1, 1, NULL),
('Acacia Creamed Honey', 350, 2, 2, 1, 2, NULL),
('Lavender Infused Honey', 250, 5, 3, 3, 5, '2024-02-20 11:30:00'),
('Pure Honeycomb', 400, 4, 4, 7, 3, NULL),
('Premium Propolis Extract', 100, 8, 5, 6, 4, NULL),
('Organic Bee Pollen', 200, 6, 6, 1, 6, '2024-01-25 14:15:00'),
('Royal Jelly Premium', 50, 5, 7, 6, 7, NULL),
('Natural Beeswax Candle Set', 150, 7, 8, 5, 8, NULL),
('Heather Honey', 500, 3, 1, 4, 9, NULL),
('Orange Blossom Honey', 250, 2, 1, 2, 10, NULL),
('Manuka Honey Special Reserve', 150, 5, 1, 3, 1, NULL),
('Buckwheat Raw Honey', 500, 3, 1, 1, 2, '2024-03-02 10:45:00'),
('Eucalyptus Honey', 350, 6, 1, 2, 3, NULL),
('Linden Honey', 250, 1, 1, 1, 4, NULL),
('Clover Honey', 500, 2, 1, 4, 5, NULL),
('Beeswax Food Wraps', NULL, 7, 8, 5, 6, NULL),
('Propolis Tincture', 30, 8, 5, 6, 7, NULL),
('Honeycomb Gift Box', 300, 7, 4, 5, 8, '2024-02-28 15:40:00'),
('Chestnut Honey', 500, 3, 1, 1, 9, NULL),
('Thyme Honey', NULL, 1, 1, 2, 10, NULL);

-- Insert data into clients
INSERT INTO clients (client_name, client_phone, client_email, client_city, client_address, client_archive) VALUES
('Johnson Grocery', '+44 20 1234 5678', 'orders@johnsongrocery.com', 'London', '15 Market Street, EC1A 1BB', NULL),
('Natural Foods Ltd', '+44 20 2345 6789', 'purchasing@naturalfoods.co.uk', 'Birmingham', '27 Organic Way, B1 1TF', NULL),
('Emma Wilson', '+44 7890 123456', 'emma.wilson@email.com', 'Manchester', '42 Bee Lane, M1 2WD', '2024-01-30 13:25:00'),
('Green Health Store', '+44 20 3456 7890', 'contact@greenhealthstore.co.uk', 'Edinburgh', '8 Wellness Road, EH1 1TH', NULL),
('James Mitchell', '+44 7901 234567', NULL, 'Glasgow', '19 Honey Avenue, G1 2QZ', NULL),
('Wholesome Market', '+44 20 4567 8901', 'orders@wholesomemarket.com', 'Bristol', '53 Pure Street, BS1 4DF', NULL),
('Sarah Johnson', '+44 7812 345678', 'sarah.j@email.com', 'Liverpool', NULL, '2024-02-25 09:15:00'),
('The Hive Café', '+44 20 5678 9012', 'manager@thehivecafe.co.uk', 'Cardiff', '22 Beekeeper Street, CF10 1DD', NULL),
('Robert Thompson', NULL, 'robert.t@email.com', 'Leeds', '31 Natural Lane, LS1 5ND', NULL),
('Organic Corner Shop', '+44 20 6789 0123', 'hello@organiccorner.co.uk', 'Newcastle', NULL, NULL);

-- Insert data into receipts
INSERT INTO receipts (receipt_total_price, receipt_date_sold, fk_client_id) VALUES
(124.50, '2024-01-15 10:23:45', 1),
(76.80, '2024-01-22 14:35:12', 3),
(198.25, '2024-01-30 09:15:27', 2),
(45.00, '2024-02-05 16:42:38', 5),
(312.75, '2024-02-12 11:30:05', 6),
(87.20, '2024-02-20 15:18:54', 4),
(156.40, '2024-02-28 10:05:32', 7),
(234.60, '2024-03-05 13:45:29', 1),
(67.90, '2024-03-10 09:22:17', 8),
(189.30, '2024-03-12 14:55:03', 10);

-- Insert data into receipts_products
INSERT INTO receipts_products (receipt_product_quantity, receipt_product_unity_price, fk_product_id, fk_receipt_id) VALUES
(3, 18.50, 1, 1),
(2, 34.50, 11, 1),
(1, 24.50, 5, 2),
(2, 26.15, 6, 2),
(5, 18.50, 1, 3),
(2, 32.75, 13, 3),
(3, 15.00, 20, 4),
(10, 18.50, 1, 5),
(5, 14.75, 9, 5),
(2, 19.50, 12, 5),
(NULL, 15.90, 15, 6),
(2, 19.75, 17, 6),
(5, 16.80, 4, 7),
(3, 23.40, 18, 7),
(8, 18.50, 1, 8),
(4, 21.90, 19, 8),
(2, NULL, 11, 9),
(6, 17.50, 14, 10),
(4, 22.45, 16, 10);