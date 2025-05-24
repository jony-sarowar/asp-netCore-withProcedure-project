using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessCoreEvi.Migrations
{
    /// <inheritdoc />
    public partial class spUpdateMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TYPE EditParamFacilityType AS TABLE(
        FacilityName NVARCHAR(50)
    );
   GO
   CREATE OR ALTER PROCEDURE dbo.spUpdateMember
    @Name NVARCHAR(50),
    @Date DATE,
    @Mobile NVARCHAR(15),
    @RoomId INT,                
    @IsPermanent BIT,
    @Amount DECIMAL(18,4),
    @ImgUrl NVARCHAR(100),
    @Facilities EditParamFacilityType READONLY,
    @MemberId INT
    AS
    BEGIN
    BEGIN TRY
    DECLARE @LocalFacility TABLE(
       FacilityName NVARCHAR(50),
       MemberId INT
     );
    UPDATE dbo.Members
    SET
    Name = @Name,
    Date = @Date,
    Mobile = @Mobile,
    RoomId = @RoomId,
    IsPermanent = @IsPermanent,
    Amount = @Amount,
    ImgUrl = @ImgUrl
    WHERE
    MemberId = @MemberId;

    DELETE FROM dbo.Facilities
    WHERE MemberId = @MemberId;

    INSERT INTO dbo.Facilities(FacilityName, MemberId)
    SELECT FacilityName, @MemberId
    FROM @Facilities

    END TRY
    BEGIN CATCH
        Throw;
    END CATCH
    END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCUDURE IF EXISTS dbo.spUpdateMember");
            migrationBuilder.Sql("DROP TYPE IF EXISTS EditParamFacilityType");
        }
    }
}
