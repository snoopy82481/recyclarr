namespace TrashLib.Sonarr;

public interface IReleaseProfileLister
{
    void ListReleaseProfiles();
    void ListTerms(string releaseProfileId);
}
