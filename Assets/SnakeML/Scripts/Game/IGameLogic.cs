
public interface IGameLogic
{
    // Should be called once on the first frame
    void Init();

    // In the event of a reset should make states default again
    void Reset();

    // Shoulde be called once each frame or once every x time
    void Update();
}
