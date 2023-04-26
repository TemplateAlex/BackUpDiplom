import pyodbc
import datetime
import uuid
import numpy as np
import sklearn
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import scale
from DBConfiguration import DBConnection

def CreatePredictions(cursor, resultOfAllCode, nowYear, typeTable):
    for row in resultOfAllCode:
        code = row[0]
        subject = row[1]

        selectCountRowsByCode = "SELECT COUNT(*) FROM Tmp" + typeTable + "TablePoints WHERE Code ='" + code + "'"
        countOfRows = cursor.execute(selectCountRowsByCode).fetchone()[0]

        if (countOfRows > 1):
            selectPointsByCode = "SELECT Points, Year FROM Tmp" + typeTable + "TablePoints WHERE Code = '" + code + "'"
            resultAllPoints = cursor.execute(selectPointsByCode).fetchall()

            points, years = [], []

            for rowYearCode in resultAllPoints:
                points.append([rowYearCode[0]])
                years.append([int(rowYearCode[1])])

            npPoints, npYears = np.array(points), np.array(years)
            LinReg = LinearRegression()
            LinReg.fit(npYears, npPoints)

            intercept, coef = LinReg.intercept_, LinReg.coef_

            predictionPoint = coef * (int(nowYear) + 1) + intercept

            if predictionPoint[0] > 35 and subject == "Творческий экзамен/Творческий экзамен":
                point = 35
            elif predictionPoint[0] < 10 and subject == "Творческий экзамен/Творческий экзамен":
                point = 10
            elif predictionPoint[0] > 140:
                point = 140
            elif predictionPoint[0] < 50:
                point = 50
            else:
                point = predictionPoint[0]

            insert = "INSERT INTO ENT" + typeTable + "Predictions(Id, " + typeTable + "Prediction, Code, Year) VALUES('" + str(uuid.uuid4()).upper() + "', " + str(int(point)) + ", '" + code + "', '" + str(int(now.year) + 1) + "')"
            cursor.execute(insert).commit()
        elif (countOfRows == 1):
            selectPointsByCode = "SELECT Points, Year FROM Tmp" + typeTable + "TablePoints WHERE Code = '" + code + "'"
            point = cursor.execute(selectPointsByCode).fetchone()[0]

            insert = "INSERT INTO ENT" + typeTable + "Predictions(Id, " + typeTable + "Prediction, Code, Year) VALUES('" + str(uuid.uuid4()).upper() + "', " + str(point) + ", '" + code + "', '" + str(int(now.year) + 1) + "')"
            cursor.execute(insert).commit()
        else:
            insert = "INSERT INTO ENT" + typeTable + "Predictions(Id, " + typeTable + "Prediction, Code, Year) VALUES('" + str(uuid.uuid4()).upper() + "', " + str(0) + ", '" + code + "', '" + str(int(now.year) + 1) + "')"
            cursor.execute(insert).commit()
            

def DropTemporyTables(cursor, typeTable):
    tableName = "Tmp" + typeTable + "TablePoints"
    cursor.execute("DROP TABLE " + tableName).commit()



connection = DBConnection().getConnection()

cursor = connection.cursor()

selectAllCodesWithSubjects = ("""
    SELECT epc.CodeName, s.SubjectName
    FROM EducationPrCode epc
    INNER JOIN EducationProgram ep ON EducationCodeId = epc.Id
    INNER JOIN Subjects s ON s.Id = ep.SubjId
""")

resultAllCode = cursor.execute(selectAllCodesWithSubjects).fetchall()
now = datetime.datetime.now()

CreatePredictions(cursor, resultAllCode, now.year, "MIN")
CreatePredictions(cursor, resultAllCode, now.year, "AVG")
DropTemporyTables(cursor, "MIN")
DropTemporyTables(cursor, "AVG")
