//2018.09.19    关林
//数据表 检索

using DataModule;
using System.Collections.Generic;
using System.Linq;

public class GameDataTable
{
    
    public static TableEcpmData GetTableEcpmData(float ecpm)
    {
        foreach (var item in DataModuleManager._instance.TableEcpmData_Dictionary.Values)
        {
            if(ecpm <= item.MaxECPM)
            {
                return item;
            }
        }

        return null;
    }
    
    public static TableGlobalVariableData GetTableGlobalVariableData(int id)
    {
        DataModuleManager._instance.TableGlobalVariableData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableAnswerInfoData GetTableAnswerInfoData(int id)
    {
        DataModuleManager._instance.TableAnswerInfoData_Dictionary.TryGetValue(id, out var data);
        return data;
    }
    
    public static TableAudioData GetTableAudioData(int id)
    {
        DataModuleManager._instance.TableAudioData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableEffectData GetTableEffectData(int id)
    {
        DataModuleManager._instance.TableEffectData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableNetworkRequestData GetTableNetworkRequestData(int id)
    {
        DataModuleManager._instance.TableNetworkRequestData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableNetworkURLData GetTableNetworkURLData(int id)
    {
        DataModuleManager._instance.TableNetworkURLData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableTextData GetTableTextData(string id)
    {
        DataModuleManager._instance.TableTextData_Dictionary.TryGetValue(id, out var data);
        return data;
    }
    public static TableOfficialInfoData GetTableOfficialInfoData(int id)
    {
        DataModuleManager._instance.TableOfficialInfoData_Dictionary.TryGetValue(id, out var data);
        return data;
    }

    public static TableItemData GetTableItemData(int id)
    {
        DataModuleManager._instance.TableItemData_Dictionary.TryGetValue(id, out var data);
        return data;
    }
    public static TableNewbieSignData GetTableNewbieSignData(int id)
    {
        DataModuleManager._instance.TableNewbieSignData_Dictionary.TryGetValue(id, out var data);
        return data;
    }
    public static TableItemData GetTableItemTypeData(int type)
    {
        var list = DataModuleManager._instance.TableItemData_Dictionary.Values;
        foreach (var item in list)
        {
            if (item.ItemType == type)
            {
                return item;
            }
        }
        return DataModuleManager._instance.TableItemData_Dictionary[1];
    }
}
