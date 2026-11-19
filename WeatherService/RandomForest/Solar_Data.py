import pandas as pd
from sklearn.ensemble import RandomForestRegressor
import joblib

# Load dataset
data = pd.read_csv("solar_data.csv")

X = data[['irradiance', 'temperature', 'cloudCover', 'humidity']]
y = data['power']

# Train model on full data 
model = RandomForestRegressor()
model.fit(X, y)

# Save model
joblib.dump(model, "solar_model.pkl")

print(" Model trained successfully")

sample = [[750, 29, 15, 60]]
prediction = model.predict(sample)

print("Predicted Power:", prediction[0])