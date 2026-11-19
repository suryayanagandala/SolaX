from flask import Flask,request,jsonify
import joblib
import pandas as pd

app=Flask(__name__)

model=joblib.load("solar_model_v1.pkl")
@app.route('/')
def home():
    return "Flask API is running"
@app.route('/test123',methods=['POST'])
def predict():
    print("API HIT")
    try:

        # data=request.get_json()

        data = request.json

        sample=pd.DataFrame([{
             "irradiance": data["irradiance"],
            "temperature": data["temperature"],
            "cloudCover": data["cloudCover"],
            "humidity": data["humidity"],
            "hour": data["hour"]
        }])

        

        prediction=model.predict(sample)[0]

        prediction=max(1.5,prediction)

        return jsonify({
            "status":True,
            "PredictedPower":round(prediction,2)
        })
    except Exception as e:
        return jsonify({
            "status":False,
            "error":str(e)
        })
if __name__ == '__main__':
    app.run(debug=True,port=5001)