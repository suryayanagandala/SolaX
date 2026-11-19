import joblib
import pandas as pd

# Load trained model
import joblib
import pandas as pd

# Load trained model
model = joblib.load("solar_model_v1.pkl")

test_data = [
    {
        "name": "Morning (8 AM, moderate clouds)",
        "irradiance": 400,
        "hour": 8,
        "temperature": 28,
        "cloudCover": 40,
        "humidity": 65
    },
    {
        "name": "Noon Sunny (1 PM, clear sky)",
        "irradiance": 600,
        "hour": 13,
        "temperature": 32,
        "cloudCover": 10,
        "humidity": 55
    },
    {
        "name": "Noon Cloudy (1 PM, heavy clouds)",
        "irradiance": 200,
        "hour": 13,
        "temperature": 30,
        "cloudCover": 80,
        "humidity": 70
    },
    {
        "name": "Evening (5 PM)",
        "irradiance": 400,
        "hour": 17,
        "temperature": 29,
        "cloudCover": 30,
        "humidity": 60
    }
]

print(" Testing Model...\n")

for test in test_data:
    sample = pd.DataFrame([{
        "irradiance": test["irradiance"],
        "temperature": test["temperature"],
        "cloudCover": test["cloudCover"],
        "humidity": test["humidity"],
        "hour": test["hour"]
    }])

    prediction = model.predict(sample)[0]

    #  prevent negative
    prediction = max(1.5, prediction)

    print(f"{test['name']}")
    print(f"→ Predicted Power: {round(prediction, 2)} kWh\n")