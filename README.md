# Employee Time Management

A Windows desktop program for running the staff side of a multi-store franchise:
employee records, daily timesheets, leave, employment contracts, payroll hours and the
manager logins that look after them. Each store's managers see only their own staff; the
owner sees every store.

Built with C# WinForms on .NET, with a MySQL database

---

## What it does

### Logging in
- Managers log in with a 4-digit PIN. Enter logs in, and the cursor shows when the
  program is busy.
- The program connects to the database while the PIN is being typed, so logging in is quick.
- Logging out returns to the PIN screen. If a manager's login is withdrawn while they are
  using the program, they are logged out at their next save.

### Dashboard
Opens after login with the figures for the manager's store (or every store, for the owner):

- **Tiles:** employees employed today, hours worked this pay week, people on leave today,
  AWOL days this week, and contracts that have ended and need renewing.
- **The renew tile** names the employees, lists their end dates on hover, and opens the
  longest-overdue one in Employees when clicked.
- **Lists of people needing attention:**
  - timesheets not captured today
  - missed timesheets over the last week
  - long hours this week, against the contract's 45-hour week
  - probation ending soon
  - contracts ended or ending
  - incomplete employee records, such as no address or next of kin

### Employees
- Search the store's employees by name, surname or ID number, and show former employees
  as well.
- **Adding or updating an employee:**
  - personal details: name, surname, ID number, SARS number, mobile number, marital
    status, dependents
  - address and bank details
  - contract: type, start date, optional end date, department, job description, hourly
    rate, opening leave balance
  - spouse, and family contacts
- Every field is checked before saving. Problems are shown against the field at fault,
  and nothing is saved until all of them pass.
- An ID number can only belong to one employee. Employees at another store are never named.
- **Terminate** ends an employee's contract with a required reason. **Reactivate** brings a
  former employee back.
- A fixed-term contract that has passed its end date shows as "Contract ended" until it
  is renewed with a new end date or terminated.

### Capture Timesheets
- Pick a day and capture each employee's status: Worked, Off, Leave or AWOL.
- For a worked day, capture time in and out, up to two breaks, and notes.
- Times can be typed loosely: `8`, `800`, `0800` and `8:00` all mean 08:00. Breaks must
  fall inside the shift.
- **Day type:** Sundays and South African public holidays are worked out automatically,
  including the Monday after a Sunday holiday and Easter. The manager can change the day
  type for a one-off declared holiday.
- **Booked leave:** employees on booked leave that day are filled in as Leave, ready to save.
- **Existing timesheets:** if one is already saved for an employee that day, the manager
  chooses whether to update it.
- **AWOL:** marking a day AWOL records it against the employee's AWOL tally automatically.
- Saving is all or nothing: if any row has a problem, nothing is saved and the problem
  row is marked.

### View Timesheets
- Hours for today, this week (Monday to Sunday), this month or any date range. Each
  employee's days worked, hours worked, Sunday extra, public holiday extra and payable
  hours are shown.
- **Pay rules:** hours are after breaks. Sunday hours earn half extra (time and a half), and
  public holiday hours earn a whole extra (double time).
- **Sign-off sheets:** prints a sheet for each selected employee for the chosen period,
  for them to sign that their hours are correct:
  - every day of the period, with status, times, breaks, hours and notes
  - days with no timesheet marked "Not captured", and days still to come left blank
  - totals, with Sunday and public holiday extra shown separately
  - a short statement for the employee to sign, lines for corrections, and signature and
    date lines for the employee and the manager
  - one page per employee (a month fits on one page), all saved as one PDF
- **Store summary PDF:** one line per employee with a totals row, the store's totals, and
  a line for the manager to sign that the hours were checked against the sign-off sheets.

### Leave
- Each employee's absence history, which can be filtered by leave type.
- **Balances:**
  - **PTO:** 1 day earned for every 17 days worked, capped at 21. The panel shows the sum
    that produced the balance.
  - **Sick leave:** 30 days per 36-month cycle from the contract start.
  - **Maternity and AWOL:** running totals.
- Book, change or delete PTO, sick or maternity leave. A day can't be booked twice.
  Booking past a balance needs a written reason, which is kept with the absence.
- An opening PTO balance can be captured for staff who started before the program.

### Contracts
- Fills in the client's own contract PDFs for an employee, which can then be previewed,
  printed or saved as a PDF.
- **Blanks filled in:** name, ID number, address, telephone, next of kin, commencement date,
  position, hourly rate and signing date. A manager's contract gets a monthly salary
  worked out from the hourly rate.
- Blanks with nothing to fill them are left as dots for handwriting. The screen lists
  them before printing.
- **Which PDF is used:** each store has its own General and Manager contract. The PDF
  matches the employee's store, so a manager always prints their own store's contract.
- **A store whose PDF of a type is missing:** the program uses another store's PDF of that
  type. It writes this store's name, workplace, trading hours and signing town over the
  other store's, re-wrapping the lines as Word would. When the store's own PDF is added,
  it is used instead.
- See `EmployeeTimeManagement/ContractTemplates/README.txt` for where the PDFs go.

### Managers (owner only)
- Promote an employee to manager with a 4-digit PIN. PINs must be unique across every store.
- Demote a manager, which stops their PIN working. The owner can't demote themself.
- Reveal a manager's PIN when needed.

### Keyboard use
The whole program can be used without the mouse. Press **F1** on any screen for the full list.

| Key | What it does |
|---|---|
| Enter / Shift+Enter | Next / previous field (on a button, Enter presses it) |
| Down / Up in a text box | Next / previous field |
| Enter or Down in a search box | Into the list of results; Up from the top of the list goes back |
| Ctrl+1 to Ctrl+7 | Dashboard, Employees, Contracts, Capture, View Timesheets, Leave, Managers |
| Ctrl+F | The screen's search box |
| Ctrl+S | Save (Employees, Capture Timesheets, Leave) or save as PDF (Contracts) |
| Ctrl+N | New employee or new absence |
| Ctrl+P | Print the contract, or the sign-off sheets |
| Esc | Cancel the employee editor, or close a window |

On the Capture Timesheets grid, Enter moves along the row: In, Out, breaks and notes, then
the next employee.

---

## Setting up

### Requirements
- Windows with .NET Framework 4.8.
- A MySQL or MariaDB database with the program's tables. The changes made since the first
  version are in `database/migrations`; run them in order. The base tables are not included
  in this repository.

### First run
The first time the program starts, it asks for the database connection string. It tests
the connection, then saves the string encrypted with Windows DPAPI, readable only on that
computer, in `C:\ProgramData\EmployeeTimeManagement\connection.dat`. The string is never
stored in the program's folder or in this repository.

For an existing install, a `.env` file next to the program with a
`MYSQL_CONNECTION_STRING=` line is imported once, encrypted and then deleted.

### Contract PDFs
The client's contract PDFs are not in this repository. Copy them into
`EmployeeTimeManagement/ContractTemplates/<StoreID>/General.pdf` and `Manager.pdf` before
building; the build copies them next to the program.

---

## Building and testing

- Open `EmployeeTimeManagement.sln` in Visual Studio and build. NuGet restores the packages
  (PDFsharp for writing PDFs, PDFium for reading and previewing them, MySqlConnector for
  the database).
- Tests: `dotnet test EmployeeTimeManagement.Tests/EmployeeTimeManagement.Tests.csproj`.
  They cover the rules that matter most: timesheet parsing and validation, leave balances
  and bookings, public holidays, the dashboard checks, employee validation, manager
  promotion rules, sign-off totals, how contract blanks are filled and the store wording
  is swapped, and connection string storage. The tests need no database.

### How the code is organised
- `Views` – the screens, the Wimpy styling and keyboard handling.
- `Controllers` – reading and writing the database. Every query is limited to the
  logged-in manager's store.
- `Models` – the rules: validation, leave balances, pay premiums, public holidays,
  dashboard checks and contract filling. They are kept free of screens and the database
  so they can be tested on their own.
- `Database` – the encrypted connection string, connection pooling, and sending several
  queries in one trip to the server for faster screens.
