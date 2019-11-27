
# viREQ

## Anforderungen
* MySQL DB
* .Net Framework v4.7.2

## Lokale DB 
Um eine lokale DB aufzubauen, das Skript **BuildDB.sql** ausführen und einen Nutzer + Passwort in die Tabelle **nutzer** einfügen. 

## DB Connection
Die Datenbank ist in der Web.config zu definieren:

     <connectionStrings>
        <add name="DefaultConnection" connectionString="server=localhost;database=vireq;uid=root;password=passwort;" providerName="MySql.Data.MySqlClient" />
    </connectionStrings>

 
 
