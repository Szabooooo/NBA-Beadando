# NBA Database
## Funkciók

### Főoldal (`Pages/Index`)

- **Fejléc:** a csapatok, játékosok és mérkőzések száma; mindegyikre kattintva a megfelelő lista nyílik meg.
- **Liga-ranglista (League leaders):** a 10 legjobb játékos a kiválasztott statisztikában. Szűrhető a következőkre:
  - pont (PPG), lepattanó (RPG), gólpassz (APG), labdaszerzés (SPG), blokk (BPG), +/-.
  - A szűrés egyszerű linkekkel működik (pl. `/?stat=rebounds`), így JavaScript nélkül is használható és könyvjelzőzhető.
  - Az első helyezett kiemelt (arany) kártyát kap, a 2–10. helyezettek mellett egy sáv mutatja, mennyire vannak közel az elsőhöz. A negatív +/- érték piros sávval jelenik meg.
  - Minden sorban látszik a játékos csapata és a lejátszott meccsek száma (GP). A 0 értékű játékosok nem jelennek meg.
- **Tabella (Standings):** a csapatok győzelmi arány (PCT) szerint rendezve.
- **Legutóbbi meccsek:** az utolsó 5 mérkőzés eredménye, a győztes félkövérrel; kattintásra megnyílik a meccs box score-ja.

### Csapatok (`Pages/TeamPages`)

- **Lista:** név, város, győzelmek, vereségek, győzelmi arány és játékosok száma; győzelmi arány szerint rendezve.
- **Létrehozás / szerkesztés:** név és város kötelező; a győzelmek és vereségek nem lehetnek negatívak.
- **Részletek:** a csapat adatai, a teljes keret (játékosok átlagaival) és a csapat összes mérkőzése győzelem/vereség (W/L) jelöléssel.
- **Törlés:**
  - Ha a csapatnak már van rögzített mérkőzése, nem törölhető (előbb a meccseit kell törölni).
  - Ha csak játékosai vannak, az oldal figyelmeztet, hogy a játékosok is törlődnek a csapattal együtt.

### Játékosok (`Pages/PlayerPages`)

- **Lista:** név, csapat (linkkel) és a fő statisztikák.
- **Létrehozás / szerkesztés:**
  - Csak a **név** és a **csapat** kötelező; a csapat legördülő listából választható.
  - A statisztikák (PPG, RPG, APG, SPG, BPG, FG%, 3P%, FT%, TS%, +/-) opcionálisak: ha üresen maradnak, 0-ként mentődnek.
  - Szerkesztésnél a csapat megváltoztatható (pl. igazolás esetén).
- **Részletek:** a játékos összes statisztikája és a **meccsnapló** (game log): minden lejátszott meccs dátuma, ellenfele és a játékos aznapi statisztikái.
- **Törlés:** a játékos box score sorai is törlődnek vele együtt.

### Mérkőzések (`Pages/GamePages`)

- **Lista:** dátum szerint (legújabb elöl), hazai és vendég csapat, eredmény, győztes.
- **Létrehozás / szerkesztés:**
  - Dátum, hazai csapat, vendég csapat és a két csapat pontszáma.
  - **Box score:** egy csapat kiválasztásakor betöltődik a kerete, és játékosonként megadható: pont (PTS), lepattanó (REB), gólpassz (AST), labdaszerzés (STL), blokk (BLK), mezőnykosár dobott/kísérlet (FGM/FGA), hárompontos (3PM/3PA), büntető (FTM/FTA), +/-.
  - A **Played** jelölőnégyzettel jelölhető, ki játszott; bármely statisztika beírása automatikusan bejelöli. Csak a bejelölt játékosok sorai mentődnek, így aki nem játszott, annak nem romlanak az átlagai.
  - A **pont automatikusan számolódik** a dobásokból: PTS = 2 × FGM + 3PM + FTM.
  - A **csapat pontszáma automatikusan kitöltődik** a játékosok pontjainak összegéből (kézzel módosítható).
  - Szerkesztésnél a már rögzített box score előre ki van töltve.
- **Részletek / törlés:** a meccs adatai és mindkét csapat box score-ja összesítő sorral.

### Ellenőrzések (validáció)

- Egy csapat nem játszhat saját maga ellen.
- Döntetlen nem lehetséges (az NBA-ben nincs döntetlen).
- Pontszámok és statisztikák nem lehetnek negatívak (kivéve a +/-).
- Box score soronként:
  - a dobott nem lehet több, mint a kísérlet (FGM ≤ FGA, 3PM ≤ 3PA, FTM ≤ FTA);
  - a hárompontos is mezőnykosár, ezért 3PM ≤ FGM és 3PA ≤ FGA;
  - a pontnak egyeznie kell a dobásokkal: PTS = 2 × FGM + 3PM + FTM.
- A hibaüzenet az érintett mező vagy játékos sora alatt jelenik meg.

### Automatikus statisztika-számítás

Mérkőzés létrehozásakor, szerkesztésekor vagy törlésekor az érintett játékosok és csapatok statisztikái **az összes meccsük alapján újraszámolódnak**:

| Statisztika | Számítás |
|---|---|
| PPG, RPG, APG, SPG, BPG, +/- | meccsenkénti átlag |
| FG%, 3P%, FT% | összes dobott / összes kísérlet × 100 |
| TS% (True Shooting) | PTS / (2 × (FGA + 0,44 × FTA)) × 100 |
| Csapat W / L | a csapat meccsei közül a megnyert és elvesztett meccsek száma |

Az értékek egy tizedesjegyre kerekítve tárolódnak.

> A kézzel beírt játékos-statisztikák és csapatmérlegek csak addig maradnak meg, amíg a játékos/csapat nem szerepel egy meccsen. Utána a meccsadatokból számolt értékek lépnek a helyükre.

