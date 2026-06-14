using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using VehicleInspectionSystem.Web.Models;

namespace VehicleInspectionSystem.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<InspectionDto>> GetInspectionsAsync()
        {
            return await _http.GetFromJsonAsync<List<InspectionDto>>(
                "https://localhost:7065/inspections"
            ) ?? new List<InspectionDto>();
        }

        public async Task<List<InspectionDto>> SearchByPlateAsync(string plate)
        {
            return await _http.GetFromJsonAsync<List<InspectionDto>>(
                $"https://localhost:7065/inspections/search?plate={plate}"
            ) ?? new List<InspectionDto>();
        }
        public async Task<List<InspectionDto>> FilterInspectionsAsync(
    string? plate,
    string? vin,
    string? owner,
    string? brand,
    string? model,
    DateTime? dateFrom,
    DateTime? dateTo,
    string? sortBy,
    string? sortDirection)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(plate))
                query.Add($"plate={Uri.EscapeDataString(plate)}");

            if (!string.IsNullOrWhiteSpace(vin))
                query.Add($"vin={Uri.EscapeDataString(vin)}");

            if (!string.IsNullOrWhiteSpace(owner))
                query.Add($"owner={Uri.EscapeDataString(owner)}");

            if (!string.IsNullOrWhiteSpace(brand))
                query.Add($"brand={Uri.EscapeDataString(brand)}");

            if (!string.IsNullOrWhiteSpace(model))
                query.Add($"model={Uri.EscapeDataString(model)}");

            if (dateFrom.HasValue)
                query.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

            if (dateTo.HasValue)
                query.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

            if (!string.IsNullOrWhiteSpace(sortBy))
                query.Add($"sortBy={Uri.EscapeDataString(sortBy)}");

            if (!string.IsNullOrWhiteSpace(sortDirection))
                query.Add($"sortDirection={Uri.EscapeDataString(sortDirection)}");

            var url = "https://localhost:7065/inspections/filter";

            if (query.Any())
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<List<InspectionDto>>(url)
                   ?? new List<InspectionDto>();
        }
        public async Task<InspectionDto?> GetInspectionByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<InspectionDto>(
                $"https://localhost:7065/inspections/{id}"
            );
        }

        public async Task<bool> UpdateInspectionAsync(int id, InspectionDto inspection)
        {
            var response = await _http.PutAsJsonAsync(
                $"https://localhost:7065/inspections/{id}",
                inspection
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInspectionAsync(int id)
        {
            var response = await _http.DeleteAsync(
                $"https://localhost:7065/inspections/{id}"
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<DashboardDto?> GetDashboardAsync(
    DateTime? dateFrom,
    DateTime? dateTo)
        {
            var query = new List<string>();

            if (dateFrom.HasValue)
                query.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

            if (dateTo.HasValue)
                query.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

            var url = "https://localhost:7065/analytics/dashboard";

            if (query.Any())
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<DashboardDto>(url);
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>(
                "http://localhost:5093/users"
            ) ?? new List<UserDto>();
        }

        public async Task<bool> UpdateUserRoleAsync(int id, string role)
        {
            var response = await _http.PutAsJsonAsync(
                $"http://localhost:5093/users/{id}/role",
                role
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _http.DeleteAsync(
                $"http://localhost:5093/users/{id}"
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<List<DocumentDto>> GetDocumentsAsync()
        {
            return await _http.GetFromJsonAsync<List<DocumentDto>>(
                "https://localhost:7121/documents"
            ) ?? new List<DocumentDto>();
        }

        public async Task<bool> UploadDocumentAsync(
    IFormFile file,
    string title,
    string category)
        {
            if (file == null || file.Length == 0)
                return false;

            using var content = new MultipartFormDataContent();

            await using var fileStream = file.OpenReadStream();

            using var fileContent = new StreamContent(fileStream);

            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType ?? "application/octet-stream"
                );

            content.Add(fileContent, "file", file.FileName);
            content.Add(new StringContent(title ?? string.Empty), "title");
            content.Add(new StringContent(category ?? string.Empty), "category");

            var response = await _http.PostAsync(
                "https://localhost:7121/documents/upload",
                content
            );

            return response.IsSuccessStatusCode;
        }

        public string GetDownloadUrl(int id)
        {
            return $"https://localhost:7121/documents/download/{id}";
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var response = await _http.DeleteAsync(
                $"https://localhost:7121/documents/{id}"
            );

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> CreateInspectionAsync(InspectionDto inspection)
        {
            var response = await _http.PostAsJsonAsync(
                "https://localhost:7065/inspections",
                inspection
            );

            return response.IsSuccessStatusCode;
        }


        public async Task<List<AppointmentDto>> GetAppointmentsAsync(
    string? status = null,
    DateTime? dateFrom = null,
    DateTime? dateTo = null)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
                query.Add($"status={Uri.EscapeDataString(status)}");

            if (dateFrom.HasValue)
                query.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");

            if (dateTo.HasValue)
                query.Add($"dateTo={dateTo.Value:yyyy-MM-dd}");

            var url = "https://localhost:7132/appointments";

            if (query.Any())
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<List<AppointmentDto>>(url)
                   ?? new List<AppointmentDto>();
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<AppointmentDto>(
                $"https://localhost:7132/appointments/{id}"
            );
        }

        public async Task<bool> CreateAppointmentAsync(AppointmentDto appointment)
        {
            var response = await _http.PostAsJsonAsync(
                "https://localhost:7132/appointments",
                appointment
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            var response = await _http.PutAsJsonAsync(
                $"https://localhost:7132/appointments/{id}/status",
                status
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var response = await _http.DeleteAsync(
                $"https://localhost:7132/appointments/{id}"
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<AppointmentStatisticsDto?> GetAppointmentStatisticsAsync()
        {
            return await _http.GetFromJsonAsync<AppointmentStatisticsDto>(
                "https://localhost:7132/appointments/statistics"
            );
        }
        public async Task<List<ExpiringInspectionDto>> GetExpiringInspectionsAsync()
        {
            return await _http.GetFromJsonAsync<List<ExpiringInspectionDto>>(
                "https://localhost:7065/inspections/expiring?days=30"
            ) ?? new List<ExpiringInspectionDto>();
        }

        public async Task<List<ReminderDto>> GetActiveRemindersAsync()
        {
            return await _http.GetFromJsonAsync<List<ReminderDto>>(
                "https://localhost:7065/reminders/active?days=30"
            ) ?? new List<ReminderDto>();
        }

        public async Task<List<ReminderDto>> GetReadRemindersAsync()
        {
            return await _http.GetFromJsonAsync<List<ReminderDto>>(
                "https://localhost:7065/reminders/read"
            ) ?? new List<ReminderDto>();
        }

        public async Task<bool> MarkReminderAsReadAsync(int id)
        {
            var response = await _http.PutAsync(
                $"https://localhost:7065/reminders/{id}/read",
                null
            );

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> MarkReminderAsUnreadAsync(int id)
        {
            var response = await _http.PutAsync(
                $"https://localhost:7065/reminders/{id}/unread",
                null
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]> GenerateAnalyticsPdfAsync(AnalyticsReportDto report)
        {
            var response = await _http.PostAsJsonAsync(
                "https://localhost:7099/reports/analytics/pdf",
                report
            );

            if (!response.IsSuccessStatusCode)
                return Array.Empty<byte>();

            return await response.Content.ReadAsByteArrayAsync();
        }

    }

}