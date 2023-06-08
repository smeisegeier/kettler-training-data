# kettler-training-data

## about

- the aim of this repo is to process and store `.xml` based training data from kettler `SJ10X SKYLON 5`
- it comprises concurring approaches for processing: `azure-data-facory` and `net-core-webapp`

## processing

### azure-data-facory

- `.xml` files are uploaded into the `azure-data-facory` via azure blobs
- [![Azure](https://badgen.net/badge/icon/azure?icon=azure&label)](https://azure.microsoft.com) pipeline transforms xml structures into sql tables

```python
📦src
 ┣ 📂azure
 ┃ ┣ 📂dataflow
 ┃ ┃ ┣ 📜Spiro_dataflow.json
 ┃ ┃ ┗ 📜YT_flow.json
 ┃ ┣ 📂dataset
 ┃ ┃ ┣ 📜DatasetSqlOrders_out.json
 ┃ ┃ ┣ 📜DelimitedTextDataset_in.json
 ┃ ┃ ┣ 📜JsonTest.json
 ┃ ┃ ┣ 📜KettlerBlobRead.json
```

### net-core-webapp

- files are uploaded into the `net-core-webapp` using a drag and drop web UI
- data are processes with ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=flat&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white) routines

## analysis

### 1️⃣ using ![Microsoft Excel](https://img.shields.io/badge/Microsoft_Excel-217346?style=flat&logo=microsoft-excel&logoColor=white) [![PowerPivot](https://badgen.net/badge/icon/powerpivot?icon=powerpivot&label)](https://www.powerpivot.com/)

> impressions

<img src="docs/img/2023-02-05-12-58-53.png" alt="impression" width=500 border=1 /><br/>

### 2️⃣ using ![Python](https://img.shields.io/badge/python-3670A0?style=flat&logo=python&logoColor=ffdd54)

- under construction 🚧

> note: probably not. power pivot is still way too strong

## built with

### used modules

- [`dropzone.js`](https://www.dropzonejs.com/)

### used packages

```xml
  <ItemGroup>
    <PackageReference Include="AutoWrapper.Core" Version="4.5.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="7.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="7.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="DextersLabor" Version="0.9.2" />
  </ItemGroup>
```

## deployment

### plain

1. run `dotnet publish` or the `publish` task if set up
1. now there is a folder bin/publish
1. rightclick ➡️ `Deploy to web ..`
1. sign in to azure
1. follow instructions in `F1` window

### github actions

1. use the `.yaml` template here if possible
2. click through deployment center to set up the secret, so no need to manually add this to github
3. care for env variables. path strings must be set
4. database settings:
5. watch progress on github repo ➡️ `actions`

- further reading [here](https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=applevel)

## azure vault

### set up user

- azure -> server -> Azure Active Directory -> set admin
- if there is no eligible user, create a new one in Azure Active Directory
- now give permissions:
  - choose subscription
  - ➡️IAM
  - add role assignment
  - Privileged administrator roles ➡️owner
  - members ➡️choose user
- wait 2mins 😄
- now use these credentials for azure login!
- login to the target database on this server

### set roles

- web app -> Identity -> Add role assignment

  - key vault
  - sql server

- web app must have identity name `KettlerFileUploader` in this example
- login as azure admin (not the regular account!) and access sql db

```sql
CREATE USER KettlerFileUploader FROM EXTERNAL PROVIDER
ALTER ROLE db_datareader ADD MEMBER KettlerFileUploader
ALTER ROLE db_datawriter ADD MEMBER KettlerFileUploader
```

> note the name of the webapp

- now the constring can look like this. note how user/passwd is replaced

```csharp
db_con = "Server=tcp:demosqlserverxd.database.windows.net,1433;Database=DemoSqlDb;Authentication=Active Directory Managed Identity;Trusted_Connection=False;Encrypt=True;PersistSecurityInfo=True;";
```

- ⚠️this constring requires the app to run in **azure** environment, it wont run on local machine debugging

- see [here](https://www.codemag.com/Article/2107041/Eliminate-Secrets-from-Your-Applications-with-Azure-Managed-Identity)
- and [here](https://mderriey.com/2021/07/23/new-easy-way-to-use-aad-auth-with-azure-sql/)

- azure vault: read [here](https://www.loginradius.com/blog/engineering/guest-post/using-azure-key-vault-with-an-azure-web-app-in-c-sharp/)
- 💡use preferred access model (role based)
- add access to the vault from the web app identity page (reader)
- add section in `appsettings.json`
- create interface
