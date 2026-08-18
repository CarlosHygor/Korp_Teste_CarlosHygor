-- Script de Carga Inicial (Seed Data) de Produtos para a Estoque.API

INSERT INTO public.produtos ("Codigo", "Descricao", "Saldo") VALUES
('PROD-001', 'Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)', 15),
('PROD-002', 'Mouse Sem Fio Logitech MX Master 3S', 40),
('PROD-003', 'Teclado Mecânico Keychron K2 RGB', 25),
('PROD-004', 'Monitor LG UltraWide 29 IPS Full HD', 12),
('PROD-005', 'Cadeira de Escritório Ergonômica Comfy Ergofit', 8),
('PROD-006', 'Headset Gamer HyperX Cloud II Red', 30),
('PROD-007', 'Webcam Full HD Logitech C920s Pro', 20),
('PROD-008', 'SSD Kingston NV2 1TB M.2 NVMe', 50),
('PROD-009', 'Memória RAM Corsair Vengeance 16GB DDR4 3200MHz', 45),
('PROD-010', 'Processador AMD Ryzen 7 5700X', 18),
('PROD-011', 'Placa de Vídeo RTX 4060 Ventus 8GB', 10),
('PROD-012', 'Fonte Corsair CV650 650W 80 Plus Bronze', 22),
('PROD-013', 'Gabinete Gamer NZXT H5 Flow Black', 14),
('PROD-014', 'Water Cooler DeepCool LE520 240mm ARGB', 16),
('PROD-015', 'Placa-Mãe ASUS TUF Gaming B550M-Plus', 15),
('PROD-016', 'Fone de Ouvido Bluetooth Sony WH-1000XM5', 7),
('PROD-017', 'Mousepad Gamer Extra Grande 90x40cm Black', 60),
('PROD-018', 'Suporte Articulado para Monitor F80N ELG', 35),
('PROD-019', 'Filtro de Linha 8 Tomadas Clamper iClamper Energia 8', 40),
('PROD-020', 'Nobreak Intelbras Attiv 600VA 120V', 11),
('PROD-021', 'Impressora Multifuncional Epson EcoTank L3250', 9),
('PROD-022', 'Roteador Wi-Fi 6 TP-Link Archer AX12', 28),
('PROD-023', 'Switch TP-Link 8 Portas Gigabit TL-SG108', 19),
('PROD-024', 'Cabo HDMI 2.1 4K 120Hz 2 Metros', 75),
('PROD-025', 'Hub USB-C 7 em 1 Baseus Dual Type-C', 33),
('PROD-026', 'Mesa Gamer Com Regulagem de Altura Elétrica 140x70', 5),
('PROD-027', 'Luminária de Monitor Baseus i-Wok Stepless Dimming', 24),
('PROD-028', 'HD Externo Portátil Seagate Expansion 2TB', 27),
('PROD-029', 'Pendrive SanDisk Ultra Flair 64GB USB 3.0', 80),
('PROD-030', 'Adaptador Bluetooth 5.0 USB TP-Link UB500', 55),
('PROD-031', 'Microfone Condensador Fifine K669B USB', 17),
('PROD-032', 'Braço Articulado para Microfone Neewer NB-35', 21),
('PROD-033', 'Ring Light 10 Polegadas Com Tripé 1.60m', 13),
('PROD-034', 'Caixa de Som Edifier R1000T4 Bivolt 24W RMS', 10),
('PROD-035', 'Organizador de Cabos Espiral 2 Metros Preto', 100)
ON CONFLICT ("Codigo") DO NOTHING;

-- Script de Carga Inicial (Seed Data) de Chaves de Idempotência para as Notas Fiscais Fechadas
INSERT INTO public.processamentos_idempotentes ("Chave", "DataProcessamentoUtc") VALUES
('NF-0003', NOW() - INTERVAL '28 days'),
('NF-0006', NOW() - INTERVAL '25 days'),
('NF-0009', NOW() - INTERVAL '22 days'),
('NF-0012', NOW() - INTERVAL '19 days'),
('NF-0015', NOW() - INTERVAL '16 days'),
('NF-0018', NOW() - INTERVAL '13 days'),
('NF-0022', NOW() - INTERVAL '10 days'),
('NF-0025', NOW() - INTERVAL '7 days'),
('NF-0028', NOW() - INTERVAL '4 days')
ON CONFLICT ("Chave") DO NOTHING;

