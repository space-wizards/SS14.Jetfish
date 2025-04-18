--UP
CREATE OR REPLACE FUNCTION fn_incremet_version()
    RETURNS TRIGGER AS $$
BEGIN
    IF NEW."Version" != OLD."Version" THEN
        RAISE EXCEPTION 'Concurrency version error';
    END IF;
    NEW."Version" := OLD."Version" + 1;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- AccessPolicies, Role, Team, TeamMember, User, Project, Lane, Card, UploadedFile

CREATE TRIGGER "BeforeUpdateAccessPoliciesTrigger"
    BEFORE UPDATE ON "AccessPolicies"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateRoleTrigger"
    BEFORE UPDATE ON "Role"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateTeamTrigger"
    BEFORE UPDATE ON "Team"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateTeamMemberTrigger"
    BEFORE UPDATE ON "TeamMember"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateUserTrigger"
    BEFORE UPDATE ON "User"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateProjectTrigger"
    BEFORE UPDATE ON "Project"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateLaneTrigger"
    BEFORE UPDATE ON "Lane"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateCardTrigger"
    BEFORE UPDATE ON "Card"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();

CREATE TRIGGER "BeforeUpdateUploadedFileTrigger"
    BEFORE UPDATE ON "UploadedFile"
    FOR EACH ROW
EXECUTE FUNCTION fn_incremet_version();
    
-- Down

DROP TRIGGER "BeforeUpdateAccessPoliciesTrigger" ON "AccessPolicies";

DROP TRIGGER "BeforeUpdateRoleTrigger" ON "Role";

DROP TRIGGER "BeforeUpdateTeamTrigger" ON "Team";

DROP TRIGGER "BeforeUpdateTeamMemberTrigger" ON "TeamMember";

DROP TRIGGER "BeforeUpdateUserTrigger" ON "User";

DROP TRIGGER "BeforeUpdateProjectTrigger" ON "Project";

DROP TRIGGER "BeforeUpdateLaneTrigger" ON "Lane";

DROP TRIGGER "BeforeUpdateCardTrigger" ON "Card";

DROP TRIGGER "BeforeUpdateUploadedFileTrigger" ON "UploadedFile";

DROP FUNCTION fn_incremet_version;