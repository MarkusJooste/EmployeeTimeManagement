-- 0001: Leave schema migration
--
-- Expand-only: widens TBL_leave for AWOL and Override reasons, and
-- TBL_employee_contracts for the Opening Balance. Nothing is replaced or
-- backfilled; the one existing TBL_employee_contracts row and every
-- existing query are unaffected.
--
-- Apply by hand against the live database. Not run by any tooling.

ALTER TABLE TBL_leave
    MODIFY COLUMN LeaveType enum('PTO','SICK','MATERNITY','AWOL') NOT NULL,
    ADD COLUMN OverrideReason varchar(255) DEFAULT NULL AFTER Reason;

ALTER TABLE TBL_employee_contracts
    ADD COLUMN OpeningPTODays int(11) NOT NULL DEFAULT 0 AFTER HourlyRate,
    ADD COLUMN OpeningBalanceAsAt date DEFAULT NULL AFTER OpeningPTODays;
