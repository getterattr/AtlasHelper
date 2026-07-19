using ImGuiNET;
using System.Numerics;
using ImGuiVector4 = System.Numerics.Vector4;

namespace AtlasHelper.Ui;

internal static class ImGuiHelpers
{
    public static void SectionLabel(string title, string description)
    {
        ImGui.TextColored(Theme.Accent, title);
        if (!string.IsNullOrWhiteSpace(description))
        {
            ImGui.SameLine();
            ImGui.TextDisabled(description);
        }
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0f, 2f));
    }

    public static void SummaryRow(string label, string value, ImGuiVector4? color = null)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TextDisabled(label);
        ImGui.TableNextColumn();
        if (color.HasValue)
            ImGui.TextColored(color.Value, value);
        else
            ImGui.TextUnformatted(value);
    }

    public static void ReferenceTable(
        string id,
        (string Header, float Weight) col1,
        (string Header, float Weight) col2,
        (string Header, float Weight) col3,
        (string, string, string)[] rows)
    {
        if (!ImGui.BeginTable(id, 3, Theme.ReferenceTableFlags))
            return;

        ImGui.TableSetupColumn(col1.Header, ImGuiTableColumnFlags.WidthStretch, col1.Weight);
        ImGui.TableSetupColumn(col2.Header, ImGuiTableColumnFlags.WidthStretch, col2.Weight);
        ImGui.TableSetupColumn(col3.Header, ImGuiTableColumnFlags.WidthStretch, col3.Weight);
        ImGui.TableHeadersRow();

        foreach (var (a, b, c) in rows)
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TextColored(Theme.Accent, a);
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(b);
            ImGui.TableNextColumn();
            ImGui.TextWrapped(c);
        }

        ImGui.EndTable();
    }
}
