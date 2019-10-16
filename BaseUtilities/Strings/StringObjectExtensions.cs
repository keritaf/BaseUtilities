﻿/*
 * Copyright © 2016 - 2019 EDDiscovery development team
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ObjectExtensionsStrings
{
    static public bool HasChars(this string obj)
    {
        return obj != null && obj.Length > 0;
    }
    static public bool IsEmpty(this string obj)
    {
        return obj == null || obj.Length == 0;
    }

    public static string ToNullSafeString(this object obj)
    {
        return (obj ?? string.Empty).ToString();
    }

    public static string ToNANSafeString(this double obj, string format)
    {
        return (obj != double.NaN) ? obj.ToString(format) : string.Empty;
    }

    public static string ToNANNullSafeString(this double? obj, string format)
    {
        return (obj.HasValue && obj != double.NaN) ? obj.Value.ToString(format) : string.Empty;
    }

    public static string Left(this string obj, int length)      // obj = null, return "".  Length can be > string
    {
        if (obj != null)
        {
            if (length < obj.Length)
                return obj.Substring(0, length);
            else
                return obj;
        }
        else
            return string.Empty;
    }

    public static string Left(this string obj, string match, StringComparison cmp = StringComparison.CurrentCulture, bool allifnotthere = false)
    {
        if (obj != null && obj.Length > 0)
        {
            int indexof = obj.IndexOf(match, cmp);
            if (indexof == -1)
                return allifnotthere ? obj : "";
            else
                return obj.Substring(0,indexof);
        }
        else
            return string.Empty;
    }

    public static string Mid(this string obj, int start, int length = 999999)      // obj = null, return "".  Mid, start/length can be out of limits
    {
        if (obj != null)
        {
            if (start < obj.Length)        // if in range
            {
                int left = obj.Length - start;      // what is left..
                return obj.Substring(start, Math.Min(left, length));    // min of left, length
            }
        }

        return string.Empty;
    }

    public static string Mid(this string obj, string match, StringComparison cmp = StringComparison.CurrentCulture, bool allifnotthere = false)
    {
        if (obj != null && obj.Length > 0)
        {
            int indexof = obj.IndexOf(match, cmp);
            if (indexof == -1)
                return allifnotthere ? obj : "";
            else
                return obj.Substring(indexof);
        }
        else
            return string.Empty;
    }

    public static bool Contains(this string data, string comparision, StringComparison c = StringComparison.CurrentCulture)        //extend for case
    {
        return data.IndexOf(comparision, c) >= 0;
    }

    public static string Alt(this string obj, string alt)
    {
        return (obj == null || obj.Length == 0) ? alt : obj;
    }

    public static string ToNullUnknownString(this object obj)
    {
        if (obj == null)
            return string.Empty;
        else
        {
            string str = obj.ToString();
            return str.Equals("Unknown") ? "" : str.Replace("_", " ");
        }
    }

    public static string ReplaceUnderscoresNull(this object obj)
    {
        if (obj == null)
            return null;
        else
        {
            string str = obj.ToString();
            return str.Equals("Unknown") ? "" : str.Replace("_", " ");
        }
    }

    // if it starts with start, and if extra is there (configurable), replace it with replacestring..
    public static string ReplaceIfStartsWith(this string obj, string start, string replacestring = "", bool musthaveextra = true, StringComparison sc = StringComparison.InvariantCultureIgnoreCase)
    {
        if (start != null && obj.StartsWith(start, sc) && (!musthaveextra || obj.Length > start.Length))
            return replacestring + obj.Substring(start.Length).TrimStart();
        else
            return obj;
    }

    public static string FirstAlphaNumericText(this string obj)     // skip to find first alpha text ignoring whitespace
    {
        if (obj == null)
            return null;
        else
        {
            string ret = "";
            int i = 0;
            while (i < obj.Length && !char.IsLetterOrDigit(obj[i]))
                i++;

            for (; i < obj.Length; i++)
            {
                if (char.IsLetterOrDigit(obj[i]))
                    ret += obj[i];
                else if (!char.IsWhiteSpace(obj[i]) )
                    break;
            }

            return ret;

        }
    }

    public static string QuoteFirstAlphaDigit(this string obj, char quotemark = '\'')    // find first alpha text and quote it.. strange function
    {
        if (obj == null)
            return null;
        else
        {
            int i = 0;
            while (i < obj.Length && !char.IsLetter(obj[i]))
                i++;

            int s = i;

            while (i < obj.Length && ( char.IsLetterOrDigit(obj[i]) || char.IsWhiteSpace(obj[i])))
                i++;

            string ret = obj.Substring(0, s) + quotemark + obj.Substring(s, i - s) + quotemark + obj.Mid(i);
            return ret;
        }
    }

    public static string ReplaceNonAlphaNumeric(this string obj)
    {
        char[] arr = obj.ToCharArray();
        arr = Array.FindAll<char>(arr, (c => char.IsLetterOrDigit(c)));
        return new string(arr);
    }

    public static string Skip(this string s, string t, StringComparison c = StringComparison.InvariantCulture)
    {
        if (s.StartsWith(t, c))
            s = s.Substring(t.Length);
        return s;
    }

    public static string SkipIf(this string s, string t, bool cond, StringComparison c = StringComparison.InvariantCulture)
    {
        if (cond && s.StartsWith(t, c))
            s = s.Substring(t.Length);
        return s;
    }

    public static void AppendPrePad(this System.Text.StringBuilder sb, string other, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb.Append(prepad);
            sb.Append(other);
        }
    }

    public static bool AppendPrePad(this System.Text.StringBuilder sb, string other, string prefix, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb.Append(prepad);
            if (prefix.Length > 0)
                sb.Append(prefix);
            sb.Append(other);
            return true;
        }
        else
            return false;
    }

    public static string AppendPrePad(this string sb, string other, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb += prepad;
            sb += other;
        }
        return sb;
    }

    public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(str.Length * 4);

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, comparison);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, comparison);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }

    public static int FirstCharNonWhiteSpace(this string obj)
    {
        int i = 0;
        while (i < obj.Length && char.IsWhiteSpace(obj[i]))
            i++;
        return i;
    }

    public static string AddSuffixToFilename(this string file, string suffix)
    {
        return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + suffix) + System.IO.Path.GetExtension(file);
    }

    public static string SafeVariableString(this string normal)
    {
        string ret = "";
        foreach (char c in normal)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                ret += c;
            else
                ret += "_";
        }
        return ret;
    }

    public static string SafeFileString(this string normal)
    {
        normal = normal.Replace("*", "_star");      // common ones rename
        normal = normal.Replace("/", "_slash");
        normal = normal.Replace("\\", "_slash");
        normal = normal.Replace(":", "_colon");
        normal = normal.Replace("?", "_qmark");

        char[] invalid = System.IO.Path.GetInvalidFileNameChars();
        foreach( char c in invalid)
            normal = normal.Replace(c,'_'); // all others _

        return normal;
    }

    public static bool EqualsAlphaNumOnlyNoCase(this string left, string right)
    {
        left = left.Replace("_", "").Replace(" ", "").ToLowerInvariant();        // remove _, spaces and lower
        right = right.Replace("_", "").Replace(" ", "").ToLowerInvariant();
        return left.Equals(right);
    }

    public static string RemoveTrailingCZeros(this string str)
    {
        int index = str.IndexOf('\0');
        if (index >= 0)
            str = str.Substring(0, index);
        return str;
    }

    public static int ApproxMatch(this string str, string other, int min)       // how many runs match between the two strings
    {
        int total = 0;
        for (int i = 0; i < str.Length; i++)
        {
            for (int j = 0; i < str.Length && j < other.Length; j++)
            {
                if (str[i] == other[j])
                {
                    int i2 = i + 1, j2 = j + 1;

                    int count = 1;
                    while (i2 < str.Length && j2 < other.Length && str[i2] == other[j2])
                    {
                        count++;
                        i2++;
                        j2++;
                    }

                    //if ( count>1)  System.Diagnostics.Debug.WriteLine("Match " + str.Substring(i) + " vs " + other.Substring(j) + " " + count);
                    if (count >= min)   // at least this number of chars in a row.
                    {
                        total += count;
                        i += count;
                        //System.Diagnostics.Debug.WriteLine(" left " + str.Substring(i));
                    }
                }
            }
        }

        //System.Diagnostics.Debug.WriteLine("** TOTAL " + str + " vs " + other + " " + total);

        return total;
    }

    public static string Truncate(this string str, int start, int length)
    {
        int len = str.Length - start;
        if (str == null || len < 1)
            return "";
        else
        {
            if (length > len)
                length = len;
            return str.Substring(start, length);
        }
    }

    static public string WordWrap(this string input, int linelen)
    {
        String[] split = input.Split(new char[] { ' ' });

        string ans = "";
        int l = 0;
        for (int i = 0; i < split.Length; i++)
        {
            ans += split[i];
            l += split[i].Length;
            if (l > linelen)
            {
                ans += Environment.NewLine;
                l = 0;
            }
            else
                ans += " ";
        }

        return ans;
    }

    static public string StackTrace(this string trace, string enclosingfunc, int lines)
    {
        int offset = trace.IndexOf(enclosingfunc);

        string ret = "";

        if (offset != -1)
        {
            CutLine(ref trace, offset);

            while (lines-- > 0)
            {
                string l = CutLine(ref trace, 0);
                if (l != "")
                {
                    if (ret != "")
                        ret = ret + Environment.NewLine + l;
                    else
                        ret = l;
                }
                else
                    break;
            }
        }
        else
            ret = trace;

        return ret;
    }

    static public string CutLine(ref string trace, int offset)
    {
        int nloffset = trace.IndexOf(Environment.NewLine, offset);
        string ret;
        if (nloffset != -1)
        {
            ret = trace.Substring(offset, nloffset - offset);
            trace = trace.Substring(nloffset);
            if (trace.Length >= Environment.NewLine.Length)
                trace = trace.Substring(Environment.NewLine.Length);
        }
        else
        {
            ret = trace;
            trace = "";
        }

        return ret;
    }

    static public int IndexOf(this string s, string[] array, out int fi)   // in array, find one with first occurance, return which one in i
    {
        int found = -1;
        fi = -1;
        for (int av = 0; av < array.Length; av++)
        {
            int pos = s.IndexOf(array[av]);
            if (pos != -1 && (found == -1 || pos < found))
            {
                found = pos;
                fi = av;
            }
        }
        return found;
    }

    public enum StartWithResult
    {
        None,
        Keyword,
        KeywordCont
    };

    public static StartWithResult StringStartsWith(ref string s, string part, string cstr, StringComparison c = StringComparison.InvariantCultureIgnoreCase)
    {
        if (part != null && s.StartsWith(part, c))
        {
            s = s.Substring(part.Length);
            if (s.StartsWith(cstr, c))
            {
                s = s.Substring(cstr.Length);
                return StartWithResult.KeywordCont;
            }
            else
                return StartWithResult.Keyword;
        }
        else
            return StartWithResult.None;
    }

    public static string FirstWord(ref string s, char[] stopchars)
    {
        int i = 0;
        while (i < s.Length && Array.IndexOf(stopchars, s[i]) == -1)
            i++;

        string ret = s.Substring(0, i);
        s = s.Substring(i).TrimStart();
        return ret;
    }

    public static string Word(this string s, char[] stopchars)
    {
        int i = 0;
        while (i < s.Length && Array.IndexOf(stopchars, s[i]) == -1)
            i++;
        return s.Substring(0, i);
    }

    public static string Word(this string s, char[] stopchars, int number)      // word 1,2,3 etc or NULL if out of words
    {
        int startpos = 0;
        int i = 0;
        while (i < s.Length)
        {
            bool stop = Array.IndexOf(stopchars, s[i]) != -1;
            i++;
            if ( stop )
            {
                if ( --number == 0 )
                    return s.Substring(startpos, i - startpos - 1);
                else
                    startpos = i;
            }
        }

        return (number==1) ? s.Substring(startpos) : null;      // if only 1 left, the EOL is the end char, so return
    }

    public static bool IsPrefix(ref string s, string t, StringComparison c = StringComparison.InvariantCulture)
    {
        if (s.StartsWith(t, c))
        {
            s = s.Substring(t.Length);
            return true;
        }
        return false;
    }

    public static string RegExWildCardToRegular(this string value)
    {
        if (value.Contains("*") || value.Contains("?"))
            return "^" + System.Text.RegularExpressions.Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        else
            return "^" + value + ".*$";
    }

    public static bool WildCardMatch(this string value, string match)
    {
        match = match.RegExWildCardToRegular();
        return System.Text.RegularExpressions.Regex.IsMatch(value, match);
    }

    // find start, find terminate, if found replace with replace plus any intermidate text if keepafter>0 (keeping after this no of char)
    // replace can have {plural|notplural} inside it, and if plural is defined, replaced with the approriate selection

    public static string ReplaceArea(this string text , string start, string terminate , string replace , int keepafter = 0 , bool? plural = null)    
    {
        int index = text.IndexOf(start);
        if ( index >= 0 )
        {
            int endindex = text.IndexOf(terminate, index + 1);

            if ( endindex > 0 )
            {
                string insert = replace + (keepafter > 0 ? text.Mid(index + keepafter, endindex - index - keepafter) : "");

                if (plural != null)
                {
                    int pi = insert.IndexOf("{");
                    if ( pi >= 0 )
                    {
                        int pie = insert.IndexOf("}", pi+1);
                        if ( pie > 0 )
                        {
                            string[] options = insert.Mid(pi + 1, pie - pi - 1).Split('|');
                            insert = insert.Left(pi) + ((plural.Value || options.Length == 1) ? options[0] : options[1]) + insert.Substring(pie + 1);
                        }
                    }
                }

                text = text.Left(index) + insert + text.Substring(endindex + terminate.Length);
            }
        }

        return text;
    }

    static public int Lines(this string s, string lineterm = null)      // Newline standard  
    {
        if (lineterm == null)
            lineterm = Environment.NewLine;

        int count = 1;         // supposedly the fastest https://www.codeproject.com/Tips/312312/Counting-Lines-in-a-String
        int position = 0;
        while ((position = s.IndexOf(lineterm, position)) != -1)
        {
            count++;
            position++;         // Skip this occurrence!
        }
        return count;
    }

    static public string LineLimit(this string s, int limit, string cutindicator)   // love extension methods
    {
        int count = 1;
        int position = 0;
        while ((position = s.IndexOf(Environment.NewLine, position)) != -1)
        {
            if (count == limit)
            {
                return s.Substring(0, position) + cutindicator;
            }

            count++;
            position++;         // Skip this occurrence!
        }

        return s;
    }

    static public string LineNumbering(this string s, int start, string fmt = "N", string newline = null)   // love extension methods
    {
        if (newline == null)
            newline = Environment.NewLine;

        StringBuilder sb = new StringBuilder();
        int position = 0, positions = 0;
        while ((positions = s.IndexOf(newline, position)) != -1)
        {
            sb.Append(start.ToStringInvariant(fmt));
            sb.Append(':');
            sb.Append(s.Substring(position, positions - position));
            sb.Append(newline);
            position = positions + newline.Length;
            start++;
        }

        if (position < s.Length)
            sb.Append(s.Substring(position));

        return sb.ToNullSafeString();
    }

    static public string LineTextInsersion(this string s, string insertatlinestart, string insertafternewline = "", string newline = null)   
    {
        if (newline == null)
            newline = Environment.NewLine;

        StringBuilder sb = new StringBuilder();
        int position = 0, positions = 0;
        while ((positions = s.IndexOf(newline, position)) != -1)
        {
            sb.Append(insertatlinestart);
            sb.Append(s.Substring(position, positions - position));
            sb.Append(newline);
            sb.Append(insertafternewline);
            position = positions + newline.Length;
        }

        if (position < s.Length)
        {
            sb.Append(insertatlinestart);
            sb.Append(s.Substring(position));
            sb.Append(newline);
            sb.Append(insertafternewline);
        }

        return sb.ToNullSafeString();
    }

    static public string Lines(this string s, int start, int count, string lineformat = null, int markline = -1)   // start = 1 to N, if less than 1, removes it off count. (so -1 count 3 only produces line 1 (-1,0 removed))
    {
        string ret = string.Empty;
        if (start < 1)
            count += start - 1;
        if (count < 1)
            return ret;

        int position = 0, newposition;
        int lineno = 0;

        while ((newposition = s.IndexOf(Environment.NewLine, position)) != -1)
        {
            lineno++;
            if (lineno >= start)
            {
                if (markline >= 1)
                    ret += (lineno == markline) ? ">>> " : "";
                if (lineformat != null)
                    ret += lineno.ToStringInvariant(lineformat) + ": ";
                ret += s.Substring(position, newposition - position) + Environment.NewLine;
                if (--count <= 0)
                    return ret;
            }

            position = newposition + Environment.NewLine.Length;
        }

        return ret;
    }


    // find the next instance of one of the chars in set, in str, and return it in res. Return string after it.  Null if not found 
    static public string NextOneOf(this string str, char[] set, out char res)
    {
        res = char.MinValue;
        int i = str.IndexOfAny(set);
        if (i >= 0)
        { 
            res = str[i];
            return str.Substring(i + 1);
        }

        return null;
    }

    static public int? ToHex(this char c)
    {
        if (char.IsDigit(c))
            return c - '0';
        else if ("ABCDEF".Contains(c))
            return c - 'A' + 10;
        else if ("abcdef".Contains(c))
            return c - 'a' + 10;
        else
            return null;
    }

    // Format into lines, breaking at linelimit.
    public static string FormatIntoLines(this IEnumerable<string> list, int linelimit = 80)     
    {
        StringBuilder res = new StringBuilder();
        int lastlf = 0;

        foreach (var s in list)
        {
            if (res.Length != lastlf)
                res.Append(",");
            res.Append(s);
            if (res.Length - lastlf >= linelimit)
            {
                res.Append(Environment.NewLine);
                lastlf = res.Length;
            }
        }

        return res.ToNullSafeString();
    }


}

