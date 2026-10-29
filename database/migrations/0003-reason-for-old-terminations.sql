-- 0003: Give old terminations a reason for ending
--
-- Until this release Terminate allowed an empty reason, and nothing else wrote
-- EndDate, so every contract with an end date and no reason is a termination.
-- The app now reads "end date, no reason" as a lapsed fixed-term contract, so
-- these rows need a reason or they show up as contracts to renew.
--
-- Apply by hand BEFORE deploying the build that adds fixed-term end dates.

UPDATE TBL_employee_contracts
SET ReasonForEnding = 'Terminated (reason not recorded)'
WHERE EndDate IS NOT NULL
  AND COALESCE(TRIM(ReasonForEnding), '') = '';
