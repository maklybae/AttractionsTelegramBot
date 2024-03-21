namespace DataManager.Mapping;

/// <summary>
/// Represents various states or statuses related to chat interactions.
/// </summary>
public enum ChatStatus
{
    WAIT_COMMAND,

    // Selection related.
    WAIT_FILE_SELECTION_OPTION,
    WAIT_FILE_SELECTION_LOADING,
    CHOOSE_SELECTION_FIELDS,
    CHOOSE_SELECTION_PARAMS,
    WAIT_SELECTION_SAVING_TYPE,
    WAIT_SELECTION_FILENAME,

    // Sorting related.
    WAIT_FILE_SORTING_OPTION,
    WAIT_FILE_SORTING_LOADING,
    CHOOSE_SORTING_FIELDS,
    CHOOSE_SORTING_PARAMS,
    WAIT_SORTING_SAVING_TYPE,
    WAIT_SORTING_FILENAME
}
