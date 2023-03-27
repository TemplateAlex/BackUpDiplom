import pyodbc
import uuid

connection = pyodbc.connect('Driver={ODBC Driver 17 for SQL Server};'
                            'Server=(localdb)\MSSQLLocalDB;'
                            'Database=DBDiplom;')

cursor = connection.cursor()

sqlInsert = ("""
    insert into ENTScoresByYear(Id, Points, Code, Year)
    values('adb', 112, 'B057', '2019')
""")

sqlInsert = "INSERT INTO ENTScoresByYear(Id, Points, Code, Year) values('" + str(uuid.uuid4()).upper() + "', 234, 'B057', '2019')"

cursor.execute(sqlInsert)

cursor.commit()

connection.close()
