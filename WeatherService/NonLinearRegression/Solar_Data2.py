import pandas as pd
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import PolynomialFeatures
from sklearn.pipeline import make_pipeline
import joblib

# Load data
data = pd.read_csv("Solar_Data2.csv")

# Features
X = data[["irradiance", "temperature", "cloudCover","humidity", "hour",]]

# Target
y = data["power"]

#  Polynomial model
model = make_pipeline(PolynomialFeatures(degree=2),LinearRegression())

model.fit(X, y)

joblib.dump(model, "solar_model_v1.pkl")

print("Polynomial model trained ")