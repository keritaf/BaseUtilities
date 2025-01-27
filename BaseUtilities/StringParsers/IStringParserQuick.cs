﻿/*
 * Copyright © 2018-2020 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using QuickJSON;

namespace BaseUtils
{
    // Quick string parsers implement..

    public interface IStringParserQuick
    {
        int Position { get; }
        string Line { get; }
        void SkipSpace();
        bool IsEOL();       // function as it can have side effects
        char GetChar();       // minvalue if at EOL.. 
        char PeekChar();       // minvalue if at EOL.. 
        bool IsStringMoveOn(string s);
        bool IsCharMoveOn(char t, bool skipspace = true);
        void BackUp();
        int NextQuotedString(char quote, char[] buffer, bool replaceescape = false);
        JToken JNextNumber(bool sign);
    }
}
