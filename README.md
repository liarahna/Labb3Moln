# Labb3Moln

Min klass heter LotrFunctions och är en Azure Function som hanterar HTTP-anrop för att arbeta med Sagan om Ringen-karaktärer.

Först läser den in LotrServices för att kunna hämta och lägga till data i min MongoDB-databas. I LotrServices har jag skapat en Get, GetById och en Create-endpoint.

Anslutningssträngen är kopplad till MongoDB från min miljövariabel MongoDbConnection. Jag använder en databas som heter lotr.

Sedan kommer min Funktion "GetLotr".

Denna funktion körs med hjälp av en HttpTrigger när API:et anropas via GET eller POST.

Om metoden är GET hämtar den alla karaktärer från databasen via GetLotrAsync.

Om metoden är POST kontrollerar den att ID, namn och typ finns och inte är tomma.
Sedan kontrollerar den om en karaktär med samma ID redan finns. Om allt är korrekt sparas karaktären i databasen.

Om metoden är något annat än GET eller POST talar den om att endast GET och POST är metoder som man får använda.

Testa i Postman eller Azure functions, använd dig utav GET för att lista allting som finns i min databas.
Vill du lägga till något i databasen, använd dig utav POST med denna mall:

{
    "Id": "5",
    "Name": "Frodo Baggins",
    "Type": "Hobbit"
}

