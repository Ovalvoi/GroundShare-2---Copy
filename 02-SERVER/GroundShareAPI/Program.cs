var builder = WebApplication.CreateBuilder(args);

// הוספת שירותים לקונטיינר
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------------------
// הגדרת CORS: מאפשר לצד הלקוח (דפדפן) לשלוח בקשות לשרת
// ללא חסימות אבטחה כשהם רצים על פורטים שונים
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// הגדרת ה-HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ---------------------------------------------------------
// אפשור קבצים סטטיים: מאפשר גישה לקבצים בתיקיית wwwroot
// זה קריטי כדי שנוכל להציג את התמונות שהועלו
// ---------------------------------------------------------
app.UseStaticFiles();

// ---------------------------------------------------------
// הפעלת ה-CORS שהוגדר למעלה
// ---------------------------------------------------------
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();