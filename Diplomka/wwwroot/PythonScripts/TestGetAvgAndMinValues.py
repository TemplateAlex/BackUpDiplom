import pyodbc
import uuid

connection = pyodbc.connect('Driver={ODBC Driver 17 for SQL Server};'
                            'Server=(localdb)\MSSQLLocalDB;'
                            'Database=DBDiplom;')

cursor = connection.cursor()

years = ["2019", "2021", "2022"]

for year in years:
    sqlSelect = "SELECT AVG(Points) as AVG, Code FROM ENTScoresByYear WHERE Year = '" + year + "' GROUP BY Code"
    cursor.execute(sqlSelect)
    resultAVG = cursor.fetchall()

    for row in resultAVG:
        idStr, avgPoints, code = str(uuid.uuid4()).upper(), row[0], row[1]
        insert = "INSERT INTO TmpAverageTablePoints(Id, Points, Code, Year) VALUES('" + idStr + "', " + str(avgPoints) + ", '" + code + "', '" + year + "')"
        cursor.execute(insert)
        cursor.commit()


    sqlSelect = "SELECT MIN(Points) as MIN, Code FROM ENTScoresByYear WHERE Year = '" + year + "' GROUP BY Code"
    cursor.execute(sqlSelect)
    resultMIN = cursor.fetchall()

    for row in resultMIN:
        idStr, minPoints, code = str(uuid.uuid4()).upper(), row[0], row[1]
        insert = "INSERT INTO TmpMinimumTablePoints(Id, Points, Code, Year) VALUES('" + idStr + "', " + str(minPoints) + ", '" + code + "', '" + year + "')"
        cursor.execute(insert)
        cursor.commit()
        



