namespace Project_Sem3.Helper.BaseRate;

public class BaseRate
{
    // Base rates cố định -> dùng const hoặc static readonly
    private const decimal BASE_RATE_VEHICLE = 5000000.0m;
    private const decimal BASE_RATE_PROPERTY = 20000000.0m;
    private const decimal BASE_RATE_HEALTH = 6000000.0m;
    public decimal BaseRateLife(int age)
    {
        if (age <= 0)
        {
            return 0;
        }

        if ( age <= 20)
        {
            return 5000000.0m;
        }

        else if (age <= 50)
        {
            return 8000000.0m;
        }

        return 10000000.0m;
    }

    public decimal BaseRateVehicle() => BASE_RATE_VEHICLE;
    public decimal BaseRateProperty() => BASE_RATE_PROPERTY;
    public decimal BaseRateHealth() => BASE_RATE_HEALTH;
}