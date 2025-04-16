using System.Text;

namespace SS14.Jetfish.Security.Model;

// When adding new values, ensure that none of the assigned values are changed, DB stores only the numerical value.
public enum AccessArea : short
{
    // Assign every "area" with its own 10th digit.

    AdminReadGlobalPolicies = 1,
    AdminWriteGlobalPolicies = 2,

    TeamCreate = 10,
    TeamEdit = 11,
    TeamDelete = 12,

    ProjectRead = 20,
}

public static class AccessAreaExtensions
{
    public static string GetPolicyNames(params AccessArea[] accessAreas)
    {
        var returnValue = new StringBuilder();

        for (var index = 0; index < accessAreas.Length; index++)
        {
            var accessArea = accessAreas[index];

            var name = Enum.GetName(accessArea.GetType(), accessArea);
            returnValue.Append(name);
            if (index < accessAreas.Length - 1)
                returnValue.Append(';');
        }

        return returnValue.ToString();
    }
}