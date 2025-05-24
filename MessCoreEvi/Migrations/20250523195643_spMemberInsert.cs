using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessCoreEvi.Migrations
{
    /// <inheritdoc />
    public partial class spMemberInsert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TYPE FParamFacilityType AS TABLE(
        FacilityName NVARCHAR(50)
    );
   GO
   CREATE OR ALTER PROCEDURE dbo.spInsertMember
    @Name NVARCHAR(50),
    @Date DATE,
    @Mobile NVARCHAR(15),
    @RoomId INT,                
    @IsPermanent BIT,
    @Amount DECIMAL(18,4),
    @ImgUrl NVARCHAR(100),
    @Facilities FParamFacilityType READONLY
    AS
    BEGIN
    BEGIN TRY
    DECLARE @LocalFacility TABLE(
       FacilityName NVARCHAR(50),
       MemberId INT
     );
     DECLARE @MemberId INT;
    INSERT INTO dbo.Members(Name, Date,Mobile,RoomId,IsPermanent,Amount,ImgUrl) VALUES (@Name,@Date,@Mobile,@RoomId,@IsPermanent,@Amount, @ImgUrl);
    SET @MemberId=SCOPE_IDENTITY();

    INSERT INTO @LocalFacility(FacilityName, MemberId)
    SELECT FacilityName, @MemberId
    FROM @Facilities

    INSERT INTO dbo.Facilities(FacilityName, MemberId)
    SELECT FacilityName, @MemberId
    FROM @LocalFacility

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
            migrationBuilder.Sql("DROP PROCUDURE IF EXISTS dbo.spInsertMember");
            migrationBuilder.Sql("DROP TYPE IF EXISTS FParamFacilityType");
        }
    }
}
