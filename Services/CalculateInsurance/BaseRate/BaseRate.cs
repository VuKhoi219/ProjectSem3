namespace Project_Sem3.Helper.BaseRate;

public class BaseRate
{
    // Base rates cố định -> dùng const hoặc static readonly
    private const decimal BASE_RATE_VEHICLE = 0.0005m;
    private const decimal BASE_RATE_PROPERTY = 500000.0m;
    private const decimal BASE_RATE_HEALTH = 10000.0m;
    private const decimal BASE_RATE_LIFE = 500000.0m;
    public decimal BaseRateLife() => BASE_RATE_LIFE;


    public decimal BaseRateVehicle(decimal coverAmount) => BASE_RATE_VEHICLE * coverAmount;
    public decimal BaseRateProperty() => BASE_RATE_PROPERTY;
    public decimal BaseRateHealth() => BASE_RATE_HEALTH;
}
