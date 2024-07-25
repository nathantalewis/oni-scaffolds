using System;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

[Serializable]
[RestartRequired]
[ConfigFile(SharedConfigLocation: true)]
[ModInfo("https://www.github.com/nathantalewis/oni-scaffolds")]
public class ScaffoldsSettings
{
  [Option("Permanent by Default", "If true, new scaffolds will be permanent by default.", "Removal")]
  [JsonProperty]
  public bool PermanentByDefault { get; set; }

  [Option("Duration", "How many cycles each scaffold lasts before it self destructs.", "Removal")]
  [Limit(0.1f, 100.0f)]
  [JsonProperty]
  public float Duration { get; set; }

  [Option("Decor Penalty Value", "How ugly Scaffolds will be. Higher values mean more of a penalty. For reference, normal Ladders have a decor penalty of 5.", "Decor Penalty")]
  [Limit(1, 50)]
  [JsonProperty]
  public int DecorPenaltyValue { get; set; }

  [Option("Decor Penalty Radius", "How many tiles away the decor penalty will be applied. For reference, normal Ladders have a radius of 1.", "Decor Penalty")]
  [Limit(1, 6)]
  [JsonProperty]
  public int DecorPenaltyRadius { get; set; }

  [Option("Upwards Speed Penalty", "How much slower dupes moving upwards on Scaffolds will be. This is a percent between 0 and 99, with 0 being no penalty and 99 preventing almost all movement.", "Speed Penalty")]
  [Limit(0, 99)]
  [JsonProperty]
  public int UpwardsSpeedPenalty { get; set; }

  [Option("Downwards Speed Penalty", "How much slower dupes moving downwards on Scaffolds will be. This is a percent between 0 and 99, with 0 being no penalty and 99 preventing almost all movement.", "Speed Penalty")]
  [Limit(0, 99)]
  [JsonProperty]
  public int DownwardsSpeedPenalty { get; set; }

  public ScaffoldsSettings()
  {
    PermanentByDefault = false;
    Duration = 10.0f;
    DecorPenaltyValue = 10;
    DecorPenaltyRadius = 2;
    UpwardsSpeedPenalty = 25;
    DownwardsSpeedPenalty = 25;
  }
}
