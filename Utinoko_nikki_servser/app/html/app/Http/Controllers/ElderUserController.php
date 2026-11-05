<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class ElderUserController extends Controller
{
    // 高齢者ユーザー取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $user = DB::table('elder_users')
                ->select('id', 'name', 'firebase_uid')
                ->where('id', $id)
                ->first();

            return response()->json($user);
        }

        $user = DB::table('elder_users')
            ->select('id', 'name', 'firebase_uid')
            ->get();

        return response()->json($user);
    }

    // 高齢者ユーザー登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255',
            'firebase_uid' => 'required|string|max:128|unique:elder_users,firebase_uid',
        ]);

        // データ登録
        $id = DB::table('elder_users')->insertGetId([
            'name' => $validated['name'],
            'firebase_uid' => $validated['firebase_uid'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したユーザーを取得
        $user = DB::table('elder_users')
            ->select('id', 'name', 'firebase_uid')
            ->where('id', $id)
            ->first();

        return response()->json($user, 201);
    }
}
