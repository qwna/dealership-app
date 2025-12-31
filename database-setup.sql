cat > database-setup.sql << 'EOF'
-- Создание таблиц для PostgreSQL
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    dealer_code VARCHAR(50) UNIQUE NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    is_admin BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS car_brands (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS car_models (
    id SERIAL PRIMARY KEY,
    brand_id INTEGER REFERENCES car_brands(id),
    name VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS dealer_data (
    id SERIAL PRIMARY KEY,
    dealer_code VARCHAR(50) NOT NULL,
    month DATE NOT NULL,
    brand_id INTEGER REFERENCES car_brands(id),
    model_id INTEGER REFERENCES car_models(id),
    cars_sold INTEGER DEFAULT 0,
    cancelled_sales INTEGER DEFAULT 0,
    cars_delivered INTEGER DEFAULT 0,
    full_name VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Начальные данные
INSERT INTO car_brands (name) VALUES 
    ('Chery'),
    ('Omoda')
ON CONFLICT DO NOTHING;

INSERT INTO car_models (brand_id, name) VALUES 
    (1, 'TIGGO4 Pro'),
    (1, 'TIGGO7 Pro'),
    (1, 'TIGGO8 Pro'),
    (2, 'C5'),
    (2, 'S5')
ON CONFLICT DO NOTHING;

-- Администратор (пароль: bomboclat)
INSERT INTO users (dealer_code, full_name, password_hash, is_admin) 
VALUES ('0000', 'Administrator', '$2a$10$N9qo8uLOickgx2ZMRZoMyeMRG3jZL7QkF5p/6Z8qL5Lt6pQY7tJXW', true)
ON CONFLICT (dealer_code) DO NOTHING;
EOF
