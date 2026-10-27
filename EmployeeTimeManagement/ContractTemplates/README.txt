Contract templates
==================

The Contracts screen fills in the client's own contract PDFs. They are not in
git, so after cloning, copy them in here yourself:

    ContractTemplates\<StoreID>\General.pdf   - general staff contract
    ContractTemplates\<StoreID>\Manager.pdf   - manager contract

<StoreID> is the store's TBL_stores.StoreID. Each store has its own copy because the store's name,
address and town are printed in the contract.

Building copies whatever PDFs are here next to EmployeeTimeManagement.exe.
A missing PDF does not break the build; the Contracts screen says which file
is missing and where it goes.
