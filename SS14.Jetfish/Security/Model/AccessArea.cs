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