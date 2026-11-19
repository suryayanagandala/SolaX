from flask import Flask, request, jsonify
import joblib

app = Flask(__name__)

# Load trained model
model = joblib.load("solar_model.pkl")

@app.route('/')
def home():
    return "Flask API is running "

@app.route('/predict', methods=['POST'])
def predict():
    if request.method == 'GET':
        return "Use POST request to get prediction "
    try:
        data = request.json

        irradiance = data['irradiance']
        temperature = data['temperature']
        cloudCover = data['cloudCover']
        humidity = data['humidity']

        prediction = model.predict([[irradiance, temperature, cloudCover, humidity]])

        return jsonify({
            "predictedPower": float(prediction[0])
        })

    except Exception as e:
        return jsonify({"error": str(e)})

if __name__ == '__main__':
    app.run(debug=True)