# SOLID — Best Practices 101

.NET 9 + Minimal API ile hazırlanmış SOLID prensipleri before/after örnekleri.

## Prensipler

| # | Prensip | Klasör |
|---|---------|--------|
| 01 | Single Responsibility (SRP) | [01-SRP](01-SRP/) |
| 02 | Open/Closed (OCP) | [02-OCP](02-OCP/) |
| 03 | Liskov Substitution (LSP) | [03-LSP](03-LSP/) |
| 04 | Interface Segregation (ISP) | [04-ISP](04-ISP/) |
| 05 | Dependency Inversion (DIP) | [05-DIP](05-DIP/) |

## Yapı

Her prensip iki gerçek dünya senaryosu içerir:

```
XX-PRENSIP/
├── README.md
├── a-SenaryoAdi/
│   ├── Before/   ← Anti-pattern
│   └── After/    ← Doğru yaklaşım
└── b-SenaryoAdi/
    ├── Before/
    └── After/
```
