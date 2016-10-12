
namespace BoardGames
{
    public static class Consts
    {
        public const int PLAYER_NAME_MAX_LENGTH = 20;
    }

    /// <summary>
    /// May be used to indicate the order of players, etc.
    /// </summary>
    public enum NumbersOrders
    {
        zeroth,
        first,
        second,
        third,
        fourth,
    }

    /// <summary>
    /// Indicates the player's position on the board.
    /// </summary>
    public enum PlayerBoardAlignment
    {
        none,
        lowindex,
        highindex
    }

    public enum MessageTypes
    {
        none,
        user_input_local,
        user_input_network,
        general_error,
        general_data,
        general_data_select_from_list,
        general_info,
        game_error,
        game_data,
        game_info,
        gamefactory_error,
        gamefactory_data,
        gamefactory_info,
        interface_error,
        interface_data,
        interface_info,
    }

    public enum DataType
    {
        none,
        input_required,
        output_message,
        output_board,
    }

}
