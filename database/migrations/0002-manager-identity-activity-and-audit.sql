-- 0002: Manager identity, activity, and audit
--
-- Expand-only: gives TBL_managers an EmployeeID, an IsActiveManager flag, and
-- the BusinessDate/CapturedBy audit pair every other table carries, and closes
-- the PIN-sharing hole with a UNIQUE index on Password. Nothing is backfilled
-- and no existing query changes.
--
-- Apply by hand against the live database. Not run by any tooling.

-- Run this first. The UNIQUE index below refuses to build over duplicate
-- PINs, so any row this reports must be resolved by hand -- reissuing one of
-- the clashing PINs -- before continuing past this point.
SELECT Password, COUNT(*) AS Holders
FROM TBL_managers
GROUP BY Password
HAVING COUNT(*) > 1;

ALTER TABLE TBL_managers
    ADD COLUMN EmployeeID int(11) DEFAULT NULL AFTER ManagerID,
    ADD COLUMN IsActiveManager tinyint(1) NOT NULL DEFAULT 1 AFTER IsAdmin,
    ADD COLUMN BusinessDate date DEFAULT NULL,
    ADD COLUMN CapturedBy int(11) DEFAULT NULL,
    ADD CONSTRAINT fk_managers_employeeid FOREIGN KEY (EmployeeID) REFERENCES TBL_employees (EmployeeID),
    ADD UNIQUE INDEX ux_managers_password (Password);
