Contract templates
==================

The Contracts screen fills in the client's own contract PDFs. They are not in
git, so after cloning, copy them in here yourself:

    ContractTemplates\<StoreID>\General.pdf   - general staff contract
    ContractTemplates\<StoreID>\Manager.pdf   - manager contract

<StoreID> is the store's TBL_stores.StoreID. Each store has its own copy because the store's name,
address and town are printed in the contract.

Until a store's own PDF of a type is here, the Contracts screen prints another
store's PDF of that type instead, with this store's name, workplace, business
hours and signing town written over the other store's. It reads those from the
store's other contract, so each store needs at least one of its two PDFs. The
screen's summary says when it is doing this; drop the store's own PDF in and it
is used from then on.

Building copies whatever PDFs are here next to EmployeeTimeManagement.exe.
A missing PDF does not break the build; the Contracts screen says which file
is missing and where it goes.
