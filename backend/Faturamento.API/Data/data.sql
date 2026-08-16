-- Script de Carga Inicial (Seed Data) de Notas Fiscais para Faturamento.API

-- Inserção de 35 Notas Fiscais (com status Aberta e Fechada)
INSERT INTO public.notas_fiscais ("Status", "DataCriacao") VALUES
('Aberta', NOW() - INTERVAL '30 days'),
('Aberta', NOW() - INTERVAL '29 days'),
('Fechada', NOW() - INTERVAL '28 days'),
('Aberta', NOW() - INTERVAL '27 days'),
('Aberta', NOW() - INTERVAL '26 days'),
('Fechada', NOW() - INTERVAL '25 days'),
('Aberta', NOW() - INTERVAL '24 days'),
('Aberta', NOW() - INTERVAL '23 days'),
('Fechada', NOW() - INTERVAL '22 days'),
('Aberta', NOW() - INTERVAL '21 days'),
('Aberta', NOW() - INTERVAL '20 days'),
('Fechada', NOW() - INTERVAL '19 days'),
('Aberta', NOW() - INTERVAL '18 days'),
('Aberta', NOW() - INTERVAL '17 days'),
('Fechada', NOW() - INTERVAL '16 days'),
('Aberta', NOW() - INTERVAL '15 days'),
('Aberta', NOW() - INTERVAL '14 days'),
('Fechada', NOW() - INTERVAL '13 days'),
('Aberta', NOW() - INTERVAL '12 days'),
('Aberta', NOW() - INTERVAL '11 days'),
('Fechada', NOW() - INTERVAL '10 days'),
('Aberta', NOW() - INTERVAL '9 days'),
('Aberta', NOW() - INTERVAL '8 days'),
('Fechada', NOW() - INTERVAL '7 days'),
('Aberta', NOW() - INTERVAL '6 days'),
('Aberta', NOW() - INTERVAL '5 days'),
('Aberta', NOW() - INTERVAL '4 days'),
('Aberta', NOW() - INTERVAL '3 days'),
('Aberta', NOW() - INTERVAL '2 days'),
('Aberta', NOW() - INTERVAL '1 days'),
('Aberta', NOW() - INTERVAL '12 hours'),
('Aberta', NOW() - INTERVAL '6 hours'),
('Aberta', NOW() - INTERVAL '3 hours'),
('Aberta', NOW() - INTERVAL '1 hours'),
('Aberta', NOW());

-- Inserção dos Itens das Notas Fiscais
INSERT INTO public.itens_nota_fiscal ("NotaFiscalId", "CodigoProduto", "DescricaoProduto", "Quantidade") VALUES
(1, 'PROD-001', 'Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)', 1),
(1, 'PROD-002', 'Mouse Sem Fio Logitech MX Master 3S', 1),

(2, 'PROD-003', 'Teclado Mecânico Keychron K2 RGB', 2),
(2, 'PROD-004', 'Monitor LG UltraWide 29 IPS Full HD', 1),

(3, 'PROD-005', 'Cadeira de Escritório Ergonômica Comfy Ergofit', 1),

(4, 'PROD-006', 'Headset Gamer HyperX Cloud II Red', 2),
(4, 'PROD-007', 'Webcam Full HD Logitech C920s Pro', 1),

(5, 'PROD-008', 'SSD Kingston NV2 1TB M.2 NVMe', 3),
(5, 'PROD-009', 'Memória RAM Corsair Vengeance 16GB DDR4 3200MHz', 2),

(6, 'PROD-010', 'Processador AMD Ryzen 7 5700X', 1),
(6, 'PROD-015', 'Placa-Mãe ASUS TUF Gaming B550M-Plus', 1),

(7, 'PROD-011', 'Placa de Vídeo RTX 4060 Ventus 8GB', 1),
(7, 'PROD-012', 'Fonte Corsair CV650 650W 80 Plus Bronze', 1),

(8, 'PROD-013', 'Gabinete Gamer NZXT H5 Flow Black', 1),
(8, 'PROD-014', 'Water Cooler DeepCool LE520 240mm ARGB', 1),

(9, 'PROD-016', 'Fone de Ouvido Bluetooth Sony WH-1000XM5', 1),

(10, 'PROD-017', 'Mousepad Gamer Extra Grande 90x40cm Black', 3),
(10, 'PROD-018', 'Suporte Articulado para Monitor F80N ELG', 2),

(11, 'PROD-019', 'Filtro de Linha 8 Tomadas Clamper iClamper Energia 8', 4),

(12, 'PROD-020', 'Nobreak Intelbras Attiv 600VA 120V', 1),

(13, 'PROD-021', 'Impressora Multifuncional Epson EcoTank L3250', 1),
(13, 'PROD-024', 'Cabo HDMI 2.1 4K 120Hz 2 Metros', 2),

(14, 'PROD-022', 'Roteador Wi-Fi 6 TP-Link Archer AX12', 2),
(14, 'PROD-023', 'Switch TP-Link 8 Portas Gigabit TL-SG108', 1),

(15, 'PROD-025', 'Hub USB-C 7 em 1 Baseus Dual Type-C', 2),

(16, 'PROD-026', 'Mesa Gamer Com Regulagem de Altura Elétrica 140x70', 1),
(16, 'PROD-027', 'Luminária de Monitor Baseus i-Wok Stepless Dimming', 1),

(17, 'PROD-028', 'HD Externo Portátil Seagate Expansion 2TB', 2),

(18, 'PROD-029', 'Pendrive SanDisk Ultra Flair 64GB USB 3.0', 5),

(19, 'PROD-030', 'Adaptador Bluetooth 5.0 USB TP-Link UB500', 3),

(20, 'PROD-031', 'Microfone Condensador Fifine K669B USB', 1),
(20, 'PROD-032', 'Braço Articulado para Microfone Neewer NB-35', 1),

(21, 'PROD-033', 'Ring Light 10 Polegadas Com Tripé 1.60m', 2),

(22, 'PROD-034', 'Caixa de Som Edifier R1000T4 Bivolt 24W RMS', 1),

(23, 'PROD-035', 'Organizador de Cabos Espiral 2 Metros Preto', 5),

(24, 'PROD-001', 'Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)', 1),
(24, 'PROD-008', 'SSD Kingston NV2 1TB M.2 NVMe', 1),

(25, 'PROD-002', 'Mouse Sem Fio Logitech MX Master 3S', 2),
(25, 'PROD-003', 'Teclado Mecânico Keychron K2 RGB', 1),

(26, 'PROD-006', 'Headset Gamer HyperX Cloud II Red', 1),

(27, 'PROD-009', 'Memória RAM Corsair Vengeance 16GB DDR4 3200MHz', 2),

(28, 'PROD-012', 'Fonte Corsair CV650 650W 80 Plus Bronze', 1),
(28, 'PROD-015', 'Placa-Mãe ASUS TUF Gaming B550M-Plus', 1),

(29, 'PROD-018', 'Suporte Articulado para Monitor F80N ELG', 1),

(30, 'PROD-024', 'Cabo HDMI 2.1 4K 120Hz 2 Metros', 3),

(31, 'PROD-001', 'Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)', 1),
(31, 'PROD-004', 'Monitor LG UltraWide 29 IPS Full HD', 1),

(32, 'PROD-007', 'Webcam Full HD Logitech C920s Pro', 1),
(32, 'PROD-031', 'Microfone Condensador Fifine K669B USB', 1),

(33, 'PROD-010', 'Processador AMD Ryzen 7 5700X', 1),
(33, 'PROD-011', 'Placa de Vídeo RTX 4060 Ventus 8GB', 1),

(34, 'PROD-017', 'Mousepad Gamer Extra Grande 90x40cm Black', 2),
(34, 'PROD-002', 'Mouse Sem Fio Logitech MX Master 3S', 1),

(35, 'PROD-003', 'Teclado Mecânico Keychron K2 RGB', 1),
(35, 'PROD-006', 'Headset Gamer HyperX Cloud II Red', 1);
